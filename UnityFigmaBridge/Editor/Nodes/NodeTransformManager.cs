﻿using UnityEngine;
using UnityFigmaBridge.Editor.FigmaApi;

namespace UnityFigmaBridge.Editor.Nodes
{
    public static class NodeTransformManager
    {
        /// <summary>
        /// Applies the transform of a Figma Node to a Unity RectTransform.
        /// </summary>
        /// <param name="targetRectTransform"></param>
        /// <param name="figmaNode"></param>
        /// <param name="figmaParentNode"></param>
        /// <param name="centerPivot"></param>
        public static void ApplyFigmaTransform(RectTransform targetRectTransform, Node figmaNode, Node figmaParentNode,
            bool centerPivot)
        {
            // Default to top left alignment
            targetRectTransform.anchorMin = targetRectTransform.anchorMax = new Vector2(0, 1);
            targetRectTransform.pivot = new Vector2(0, 1);

            if (figmaNode.relativeTransform != null)
            {
                // Apply the "relativeTransform" from Figma Node for translation and rotation
                targetRectTransform.anchoredPosition = new Vector2(figmaNode.relativeTransform[0, 2],
                    -figmaNode.relativeTransform[1, 2]);
                var rotation = Mathf.Rad2Deg *
                               Mathf.Atan2(-figmaNode.relativeTransform[1, 0], figmaNode.relativeTransform[0, 0]);
                targetRectTransform.localRotation = Quaternion.Euler(0, 0, rotation);
            }

            // Apply the "size" figmaNode from figma to set size
            targetRectTransform.sizeDelta = new Vector2(figmaNode.size.x, figmaNode.size.y);


            // Apply constraints
            // Groups in Figma dont have their own constraints, so to setup effectively, we need to use the first child's constraints
            var useFirstChildForConstraints = figmaNode.type == NodeType.GROUP && figmaNode.children != null &&
                                              figmaNode.children.Length > 0;
            var constraintsSourceNode = useFirstChildForConstraints ? figmaNode.children[0] : figmaNode;

            ApplyFigmaConstraints(targetRectTransform, constraintsSourceNode, figmaParentNode);
            
            // We'll also use these properties to apply pivot after, where required
            if (centerPivot) SetPivot(targetRectTransform, new Vector2(0.5f, 0.5f));
        }
    
        /// <summary>
        /// Applies constraints to a given RectTransform based on a given Figma Node
        /// </summary>
        /// <param name="targetRectTransform"></param>
        /// <param name="figmaNode"></param>
        /// <param name="figmaParentNode"></param>
        private static void ApplyFigmaConstraints(RectTransform targetRectTransform, Node figmaNode,Node figmaParentNode)
        {
             // Setup anchor positions 
            (targetRectTransform.anchorMin, targetRectTransform.anchorMax) = AnchorPositionsForFigmaConstraints(figmaNode.constraints);
            
            // We'll need to use the size of the parent node to determine anchor position
            var parentNodeSize = figmaParentNode.size != null ? figmaParentNode.size : new Vector { x = 0, y = 0 };
    
            // TODO - Implement SCALE constraint
            
            // Modify anchor position according to constraint
            var anchoredPosition = targetRectTransform.anchoredPosition;

            anchoredPosition.x += figmaNode.constraints.horizontal switch
            {
                LayoutConstraint.HorizontalLayoutConstraint.CENTER => -parentNodeSize.x * 0.5f,
                LayoutConstraint.HorizontalLayoutConstraint.RIGHT => -parentNodeSize.x,
                _ => 0
            };

            anchoredPosition.y += figmaNode.constraints.vertical switch
            {
                LayoutConstraint.VerticalLayoutConstraint.CENTER => parentNodeSize.y * 0.5f,
                LayoutConstraint.VerticalLayoutConstraint.BOTTOM => parentNodeSize.y,
                _ => 0
            };
            
            targetRectTransform.anchoredPosition = anchoredPosition;

            switch (figmaNode.constraints.horizontal)
            {
                case LayoutConstraint.HorizontalLayoutConstraint.LEFT_RIGHT: 
                case LayoutConstraint.HorizontalLayoutConstraint.SCALE:
                    var sizeDelta = targetRectTransform.sizeDelta;
                    targetRectTransform.sizeDelta = new Vector2(sizeDelta.x-parentNodeSize.x, sizeDelta.y);
                    break;
            }

            switch (figmaNode.constraints.vertical)
            {
                case LayoutConstraint.VerticalLayoutConstraint.TOP_BOTTOM:
                case LayoutConstraint.VerticalLayoutConstraint.SCALE:
                    var sizeDelta = targetRectTransform.sizeDelta;
                    targetRectTransform.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y - parentNodeSize.y);
                    break;
            }
            
        }

        /// <summary>
        /// Applies the transform of a Figma Node to a Unity RectTransform using absolute position. This is
        /// required for Server rendered images, that have the transform (eg rotation) baked in
        /// </summary>
        /// <param name="targetRectTransform"></param>
        /// <param name="figmaNode"></param>
        /// <param name="figmaParentNode"></param>
        /// <param name="centerPivot"></param>
        public static void ApplyAbsoluteBoundsFigmaTransform(RectTransform targetRectTransform, Node figmaNode,
            Node figmaParentNode, bool centerPivot)
        {
            // Default to top left alignment
            targetRectTransform.anchorMin = targetRectTransform.anchorMax = new Vector2(0, 1);
            targetRectTransform.pivot = new Vector2(0, 1);
            
            // We'll use absolute bounding box size
            targetRectTransform.sizeDelta = new Vector2(figmaNode.absoluteBoundingBox.width, figmaNode.absoluteBoundingBox.height);
            
            // Position will be relative to parent absoluteBoundingBox
            targetRectTransform.anchoredPosition=new Vector2(figmaNode.absoluteBoundingBox.x-figmaParentNode.absoluteBoundingBox.x,
                -(figmaNode.absoluteBoundingBox.y-figmaParentNode.absoluteBoundingBox.y));
            
            ApplyFigmaConstraints(targetRectTransform, figmaNode, figmaParentNode);
            
            // We'll also use these properties to apply pivot after, where required
            if (centerPivot) SetPivot(targetRectTransform, new Vector2(0.5f, 0.5f));
            
        }
        
        /// <summary>
        /// Modify the pivot point of a RectTransform, and move to ensure position stays the same
        /// </summary>
        /// <param name="target"></param>
        /// <param name="pivot"></param>
        private static void SetPivot(RectTransform target, Vector2 pivot)
        {
            if (!target) return;
            var offset=pivot - target.pivot;
            offset.Scale(target.rect.size);
            var worldPos= target.position + target.TransformVector(offset);
            target.pivot = pivot;
            target.position = worldPos;
        }

        /// <summary>
        /// Apply anchor positions according to constraints
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        private static (Vector2, Vector2) AnchorPositionsForFigmaConstraints(LayoutConstraint constraint)
        {
            var anchorsX = constraint.horizontal switch
            {
                LayoutConstraint.HorizontalLayoutConstraint.LEFT => new Vector2(0, 0),
                LayoutConstraint.HorizontalLayoutConstraint.RIGHT => new Vector2(1, 1),
                LayoutConstraint.HorizontalLayoutConstraint.CENTER => new Vector2(0.5f, 0.5f),
                LayoutConstraint.HorizontalLayoutConstraint.LEFT_RIGHT => new Vector2(0, 1),
                LayoutConstraint.HorizontalLayoutConstraint.SCALE => new Vector2(0, 1),
                _ => new Vector2(0, 1)
            };

            var anchorsY = constraint.vertical switch
            {
                LayoutConstraint.VerticalLayoutConstraint.TOP => new Vector2(1, 1),
                LayoutConstraint.VerticalLayoutConstraint.BOTTOM => new Vector2(0, 0),
                LayoutConstraint.VerticalLayoutConstraint.CENTER => new Vector2(0.5f, 0.5f),
                LayoutConstraint.VerticalLayoutConstraint.TOP_BOTTOM => new Vector2(0, 1),
                LayoutConstraint.VerticalLayoutConstraint.SCALE => new Vector2(0, 1),
                _ => new Vector2(0, 1)
            };
            return (new Vector2(anchorsX.x, anchorsY.x), new Vector2(anchorsX.y, anchorsY.y));
        }
    }
}