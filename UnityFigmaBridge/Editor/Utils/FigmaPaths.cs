using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityFigmaBridge.Editor.FigmaApi;
using UnityFigmaBridge.Editor.Settings;

namespace UnityFigmaBridge.Editor.Utils
{
    public static class FigmaPaths
    {
        /// <summary>
        ///  Root folder for assets
        /// </summary>
        public static string FigmaAssetsRootFolder = "Assets/Figma";
        public static string FigmaAssetsBackupFolder = "Assets/Figma/Backup";

        public static string FigmaPagePrefabBackupFolder = $"{FigmaAssetsBackupFolder}/Pages";
        public static string FigmaScreenPrefabBackupFolder = $"{FigmaAssetsBackupFolder}/Screens";
        public static string FigmaComponentPrefabBackupFolder = $"{FigmaAssetsBackupFolder}/Components";

        /// <summary>
        /// Assert folder to store page prefabs)
        /// </summary>
        public static string FigmaPagePrefabFolder = $"{FigmaAssetsRootFolder}/Pages";
        /// <summary>
        /// Assert folder to store flowScreen prefabs (root level frames on pages)
        /// </summary>
        public static string FigmaScreenPrefabFolder = $"{FigmaAssetsRootFolder}/Screens";
        /// <summary>
        /// Assert folder to store compoment prefabs
        /// </summary>
        public static string FigmaComponentPrefabFolder = $"{FigmaAssetsRootFolder}/Components";
        /// <summary>
        /// Asset folder to store image fills
        /// </summary>
        public static string FigmaImageFillFolder = $"{FigmaAssetsRootFolder}/ImageFills";
        /// <summary>
        /// Asset folder to store server rendered images
        /// </summary>
        public static string FigmaServerRenderedImagesFolder = $"{FigmaAssetsRootFolder}/ServerRenderedImages";

        /// <summary>
        /// Asset folder to store Font material presets
        /// </summary>
        public static string FigmaFontMaterialPresetsFolder = $"{FigmaAssetsRootFolder}/FontMaterialPresets";

        /// <summary>
        /// Asset folder to store Font assets (TTF and generated TMP fonts)
        /// </summary>
        public static string FigmaFontsFolder = $"{FigmaAssetsRootFolder}/Fonts";

        public static void Setup(UnityFigmaBridgeSettings figmaImportProcessDataSettings)
        {
            if (!string.IsNullOrEmpty(figmaImportProcessDataSettings.FigmaAssetRootPath))
            {
                FigmaAssetsRootFolder = figmaImportProcessDataSettings.FigmaAssetRootPath;
                FigmaPagePrefabFolder = $"{FigmaAssetsRootFolder}/Pages";
                FigmaScreenPrefabFolder = $"{FigmaAssetsRootFolder}/Screens";
                FigmaComponentPrefabFolder = $"{FigmaAssetsRootFolder}/Components";
                FigmaImageFillFolder = $"{FigmaAssetsRootFolder}/ImageFills";
                FigmaServerRenderedImagesFolder = $"{FigmaAssetsRootFolder}/ServerRenderedImages";
                FigmaFontMaterialPresetsFolder = $"{FigmaAssetsRootFolder}/FontMaterialPresets";
                FigmaFontsFolder = $"{FigmaAssetsRootFolder}/Fonts";
            }
            if (!string.IsNullOrEmpty(figmaImportProcessDataSettings.FigmaBackupAssetRootPath))
            {
                FigmaAssetsBackupFolder = figmaImportProcessDataSettings.FigmaBackupAssetRootPath;
                FigmaPagePrefabBackupFolder = $"{FigmaAssetsBackupFolder}/Pages";
                FigmaScreenPrefabBackupFolder = $"{FigmaAssetsBackupFolder}/Screens";
                FigmaComponentPrefabBackupFolder = $"{FigmaAssetsBackupFolder}/Components";
            }

            if (figmaImportProcessDataSettings.UseCustomPathForScreen)
                FigmaScreenPrefabFolder = figmaImportProcessDataSettings.CustomPathScreen;
            if (figmaImportProcessDataSettings.UseCustomPathForComponent)
                FigmaComponentPrefabFolder = figmaImportProcessDataSettings.CustomPathComponent;
            if (figmaImportProcessDataSettings.UseCustomPathForPage)
                FigmaPagePrefabFolder = figmaImportProcessDataSettings.CustomPathPage;
            if (figmaImportProcessDataSettings.UseCustomPathForFont)
                FigmaFontsFolder = figmaImportProcessDataSettings.CustomPathFont;
            if (figmaImportProcessDataSettings.UseCustomPathForServerRenderedImage)
                FigmaServerRenderedImagesFolder = figmaImportProcessDataSettings.CustomPathServerRenderedImage;
            if (figmaImportProcessDataSettings.UseCustomPathForImageFills)
                FigmaImageFillFolder = figmaImportProcessDataSettings.CustomPathImageFills;
        }

        public static string GetPathForImageFill(string imageId)
        {
            return $"{FigmaPaths.FigmaImageFillFolder}/{imageId}.png";
        }

        public static string GetPathForServerRenderedImage(string nodeId,
            List<ServerRenderNodeData> serverRenderNodeData)
        {
            var matchingEntry = serverRenderNodeData.FirstOrDefault((node) => node.SourceNode.id == nodeId);
            switch (matchingEntry.RenderType)
            {
                case ServerRenderType.Export:
                    return $"Assets/{matchingEntry.SourceNode.name}.png";
                default:
                    var safeNodeId = FigmaDataUtils.ReplaceUnsafeFileCharactersForNodeId(nodeId);
                    return $"{FigmaPaths.FigmaServerRenderedImagesFolder}/{safeNodeId}.png";

            }
        }

        public static string GetPathForScreenPrefab(Node node, int duplicateCount, bool backup = false)
        {
            string folder = backup ? FigmaScreenPrefabBackupFolder : FigmaScreenPrefabFolder;
            return GetPathForScreenPrefab(node.name, duplicateCount, backup);
        }

        public static string GetPathForScreenPrefab(string nodeName, int duplicateCount, bool backup = false)
        {
            string folder = backup ? FigmaScreenPrefabBackupFolder : FigmaScreenPrefabFolder;
            if (duplicateCount > 0) nodeName += $"_{duplicateCount}";
            nodeName = ReplaceUnsafeCharacters(nodeName);
            return $"{folder}/{nodeName}.prefab";
        }
        public static string GetPathForPagePrefab(Node node, int duplicateCount, bool backup = false)
        {
            return GetPathForPagePrefab(node.name, duplicateCount, backup);
        }

        public static string GetPathForPagePrefab(string nodeName, int duplicateCount, bool backup = false)
        {
            string folder = backup ? FigmaPagePrefabBackupFolder : FigmaPagePrefabFolder;
            if (duplicateCount > 0) nodeName += $"_{duplicateCount}";
            nodeName = ReplaceUnsafeCharacters(nodeName);
            return $"{folder}/{nodeName}.prefab";
        }

        public static string GetPathForComponentPrefab(string nodeName, int duplicateCount, bool backup = false)
        {
            // If name already used, create a unique name
            string folder = backup ? FigmaComponentPrefabBackupFolder : FigmaComponentPrefabFolder;
            if (duplicateCount > 0) nodeName += $"_{duplicateCount}";
            nodeName = ReplaceUnsafeCharacters(nodeName);
            return $"{folder}/{nodeName}.prefab";
        }

        public static string GetFileNameForNode(Node node, int duplicateCount)
        {
            var safeNodeTitle = ReplaceUnsafeCharacters(node.name);
            // If name already used, create a unique name
            if (duplicateCount > 0) safeNodeTitle += $"_{duplicateCount}";
            return safeNodeTitle;
        }

        private static string ReplaceUnsafeCharacters(string inputFilename)
        {
            // We want to trim spaces from start and end of filename, or we'll throw an error
            // We no longer want to use the final "/" character as this might be used by the user
            var safeFilename = inputFilename.Trim();
            return MakeValidFileName(safeFilename);
        }

        // From https://www.csharp-console-examples.com/general/c-replace-invalid-filename-characters/
        public static string MakeValidFileName(string name)
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            invalidChars += ".";
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "_");
        }

        public static void CreateRequiredDirectories(bool updatePrefabs)
        {
            // The new delta system only applies changes with created prefabs
            // And as we can chose to only create prefabs from selected pages
            // We should be deleting previous prefabs to reduce size

            //  Create directory for pages if required 
            if (!Directory.Exists(FigmaPagePrefabFolder)) 
                Directory.CreateDirectory(FigmaPagePrefabFolder);

            // Remove existing prefabs for pages
            foreach (var file in new DirectoryInfo(FigmaPagePrefabFolder).GetFiles()) 
                file.Delete();

            //  Create directory for flowScreen prefabs if required 
            if (!Directory.Exists(FigmaScreenPrefabFolder)) 
                Directory.CreateDirectory(FigmaScreenPrefabFolder);

            // Remove existing flowScreen prefabs
            foreach (var file in new DirectoryInfo(FigmaScreenPrefabFolder).GetFiles()) 
                file.Delete();

            if (!Directory.Exists(FigmaComponentPrefabFolder)) 
                Directory.CreateDirectory(FigmaComponentPrefabFolder);

            // Remove existing components prefabs
            foreach (var file in new DirectoryInfo(FigmaComponentPrefabFolder).GetFiles()) 
                file.Delete();

            //  Create directory for image fills if required 
            if (!Directory.Exists(FigmaImageFillFolder)) 
                Directory.CreateDirectory(FigmaImageFillFolder);
            
            if (updatePrefabs)
                foreach (var file in new DirectoryInfo(FigmaImageFillFolder).GetFiles()) 
                    file.Delete();
            
            //  Create directory for server rendered images if required 
            if (!Directory.Exists(FigmaServerRenderedImagesFolder))
                Directory.CreateDirectory(FigmaServerRenderedImagesFolder);
            
            // Remove existing server rendered images
            if (updatePrefabs)
                foreach (var file in new DirectoryInfo(FigmaServerRenderedImagesFolder).GetFiles()) 
                    file.Delete();
            
            if (!Directory.Exists(FigmaFontMaterialPresetsFolder))
                Directory.CreateDirectory(FigmaFontMaterialPresetsFolder);
            
            // Remove existing FigmaFontMaterialPresetsFolder
            if (updatePrefabs)
                foreach (var file in new DirectoryInfo(FigmaFontMaterialPresetsFolder).GetFiles()) 
                    file.Delete();
            
            if (!Directory.Exists(FigmaFontsFolder)) 
                Directory.CreateDirectory(FigmaFontsFolder);
            
            // Remove existing fonts
            if (updatePrefabs)
                foreach (var file in new DirectoryInfo(FigmaFontsFolder).GetFiles()) 
                    file.Delete();
        }

        public static void BackupPrefabs()
        {
            //  Create directory for pages if required 
            if (!Directory.Exists(FigmaPagePrefabBackupFolder))
            {
                Directory.CreateDirectory(FigmaPagePrefabBackupFolder);
            }

            // Copy existing prefabs for pages
            if (Directory.Exists(FigmaPagePrefabFolder))
            {
                foreach (var file in new DirectoryInfo(FigmaPagePrefabFolder).GetFiles())
                {
                    file.CopyTo(Path.Combine(FigmaPagePrefabBackupFolder, file.Name), true);
                }
            }

            //  Create directory for pages if required 
            if (!Directory.Exists(FigmaScreenPrefabBackupFolder))
            {
                Directory.CreateDirectory(FigmaScreenPrefabBackupFolder);
            }

            if (Directory.Exists(FigmaScreenPrefabFolder))
            {
                // Copy existing prefabs for pages
                foreach (var file in new DirectoryInfo(FigmaScreenPrefabFolder).GetFiles())
                {
                    file.CopyTo(Path.Combine(FigmaScreenPrefabBackupFolder, file.Name), true);
                }
            }

            //  Create directory for pages if required 
            if (!Directory.Exists(FigmaComponentPrefabBackupFolder))
            {
                Directory.CreateDirectory(FigmaComponentPrefabBackupFolder);
            }

            if (Directory.Exists(FigmaComponentPrefabFolder))
            {
                // Copy existing prefabs for pages
                foreach (var file in new DirectoryInfo(FigmaComponentPrefabFolder).GetFiles())
                {
                    file.CopyTo(Path.Combine(FigmaComponentPrefabBackupFolder, file.Name), true);
                }
            }
        }

        public static void DeleteBackup()
        {
            //  Create directory for pages if required 
            if (Directory.Exists(FigmaPagePrefabBackupFolder))
                Directory.Delete(FigmaPagePrefabBackupFolder, true);
            

            //  Create directory for pages if required 
            if (Directory.Exists(FigmaScreenPrefabBackupFolder))
                Directory.Delete(FigmaScreenPrefabBackupFolder, true);
            

            //  Create directory for pages if required 
            if (Directory.Exists(FigmaComponentPrefabBackupFolder))
                Directory.Delete(FigmaComponentPrefabBackupFolder, true);
            

        }
    }
}