# Change Log:
## 1.0.11
- Enhancement - Now only the resources and prefabs used in the currently selected page to import will be built or downloaded
- Enhancement - User paths for each types of Figma items
- Bug Fix - Applying properly the delta between the backup and new prefab
- Bug Fix - Adding a null check on field comparer
- Bug Fix - Missing condition to apply delta making the delta always being applied even without backup

## 1.0.10
- Enhancement - Adding an option for EmojiTextMeshProUGUI instead of TextMeshProUGUI
- Enhancement - Adding a soft-shadow prefab that can be used to make soft shadow for rectangle objects using UIEffect package
- Enhancement - Adding options to tweak TMP text handling to look closer to Figma's text
- Enhancement - Adding a way to select the path of the containing folder of the google-font.json document and save it 
- Bug Fix - Child order when reapplying changes to prefab
- Bug Fix - Choose the starting screen from the flow of the current page
- Bug Fix - Handling layout group when Figma layout was SPACE_BETWEEN
- Change - Adding an option to choose if the pivot = anchor

## 1.0.9
- Enhancement - Soft shadow for texts
- Enhancement - Support for prefab preservation 
- Enhancement - Adding a way to backup Figma prefabs 
- Enhancement - Adding custom root folder settings in the settings file
- Bug Fix - Fixing issue with Figma prefabs 

## 1.0.8

Better support for Linear Color Space (@naoya-maeda). Please note that from this release
all imported textures will have sRGB set to true. The FigmaImage component
now supports sRGB textures in both gamma and linear color spaces/

## 1.0.7

- Enhancement - Added support for "Fit" Image mode (@laura-copop)
- Enhancement - Added support for flipped nodes (@laura-copop)
- Bug fix - JSON Deserialisation no longer throws errors for missing items (@laura-copop)
- Bug fix - Correctly uses project colorspace (@laura-copop)
- Bug fix - Non-visible fills no longer default to visible (@laura-copop)
- Bug fix - Server side images now batch where required to prevent Figma API errors

## 1.0.6

- Enhancement - Added page sync selection (thanks @SatoruUeda)
- Enhancement - Implemented correct alignment for auto-layout
- Enhancement - Use Server-side rendering for boolean shapes
- Bug Fix - Fix for crash import for deeply nested components
- Change - Auto layout components are disabled by default
- Bug fix - Masked objects no longer render the masks themselves

## 1.0.5

Small release adding a few new Figma features and bugs. Thanks @SatoruUeda for the contributions and for all the reports.

- Enhancement -Added support for "Fill" Image mode
- Enhancement -Added text auto sizing
- Enhancement -Added image fill opacity support
- Enhancement -Added layer opacity support
- Bug fix - Fixed issues with components being overwritten
- Bug fix - Numerous other small bugfixes