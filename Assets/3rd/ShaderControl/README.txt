************************************
*          SHADER CONTROL          *
* (C) Copyright 2016-2020 Kronnect * 
*            README FILE           *
************************************


How to use this asset
---------------------
Firstly, you should run the Demo Scene provided to get an idea of the overall functionality.
Later, you should read the documentation and experiment with the tool.

Hint: to use the asset, open it form Assets / Browse Shaders...


Demo Scene
----------
There's one demo scene, located in "Demo" folder. Just go there from Unity, open "Demo" scene and run it.


Documentation
-------------
The PDF is located in the Documentation folder. It contains additional instructions on how to use this asset.


Support
-------
Please read the documentation PDF and browse/play with the demo scene and sample source code included before contacting us for support :-)

* Support: contact@kronnect.me
* Website-Forum: http://kronnect.me
* Twitter: @KronnectGames


Future updates
--------------

All our assets follow an incremental development process by which a few beta releases are published on our support forum (kronnect.com).
We encourage you to signup and engage our forum. The forum is the primary support and feature discussions medium.

Of course, all updates of Shader Control will be eventually available on the Asset Store.


Version history
---------------

Version 6.2
- Support for Shader Graph file format 2

Version 6.1.1
- Now, in case that a shader cannot be read or analyzed, Shader Control will emit a specific console message with the location of the shader

Version 6.1
- Usability improvements and optimizations

Version 6.0
- Ability to specify allowed shader variants (under Build View, expand shader, click "Advanced")
- Ability to export to a Shader Variant Collection

Version 5.0.4
- A message will now appear when trying to convert to local keywords in shaders that are read-only

Version 5.0.3
- Shaders belonging to internal Unity packages are now listed as read-only

Version 5.0.1
- Added count of additional keywords used by internal Unity shaders in Project View, next to keywords detected in project files.

Version 5.0
- Added Shader Graph shaders compatibility. Shader Control now detects keywords defined inside a shader graph both in Build and Project View. It also can convert to local with a single click.

Version 4.9
- Added "Not Used In Build" filter in Project View

Version 4.8
- Fixes and quick build optimizations

Version 4.7
- Added new filter "Material" to Project View with options to prune keywords from project materials

Version 4.6
- Added new filter "Modified" to Project View
- [Fix] Fixed UI issue which would not display the name of some keywords

Version 4.5
- Added a new sort by "Keyword" in Build View which lists all keywords in build and allows quick selection in Project View

Version 4.4
- Added "Select Materials In Project" and "Select Materials In Scene"
- Project Panel now shows the shader name and not the shader filename (matching Build View list)
- Improved performance of List Materials option

Version 4.3
- Added compatibility with Unity 2019.3

Version 4.2
- Shader Control now suggests converting all shader_features keywords to local with a single button (if suitable keywords are found)

Version 4.1
- Total keywords + global keywords (excluding keywords defined as local) are now shown in Project View
- Added new filters to Project View: filter by Keyword Scope (global/local) and Pragma Type (multi_compile or shader_feature)
- Added button to convert keyword to local (in Project View / Sort by Keyword)
- Added button to restore shader backup from keyword view

Version 4.0 
- Support for multi_compile_local and shader_feature_local pragma directives
- New Build View -> shows internal Unity shaders and allow finer exclusion (Unity 2018.2+ required)

Version 3.1 29-NOV-2018
- Detects readonly shader files
- [Fix] Fixed wrong classification in Shader Control window of compiled/internal Unity shaders

Version 3.0 15-OCT-2018
- Added Sort by Keyword
- Ignores potential comments in same line of #pragma multi_compile sentence

Version 2.2 1-OCT-2018
- Added "Force Scan All Shaders" option

Version 2.1.1 9-AUG-2018
- Minor internal improvements

Version 2.1 20-SEP-2017
- New Sort option: by Variants Count|Keywords Count|Name
- New filter by enabled keyword count

Version 2.0.1 20-DIC-2016
- Shaders are now sorted by variant count
- Added tooltips to the window interface

Version 2.0 14-MAR-2017
- Support for hidden shaders.
- Ability to prune material keywords when shader source is not available.
- Shows materials with no keywords but linked to shaders that expose keywords

Version 1.2 7-DEC-2016
- Support for shader features. They will show up in the keywords list so can be disabled.

Version 1.1 2-DEC-2016
- Now materials are updated at project level (previously only materials inside Resources folders were refreshed)
- New "Clean All Materials" option

Version 1.0 21-NOV-2016
First Release.
