if not exist Package\lib\sl4-wp71 mkdir Package\lib\sl4-wp71
copy Facebook.WP7.Controls\bin\Facebook.WP7.Controls.dll Package\lib\sl4-wp71\
tools\nuget.exe pack Facebook.WP7.Controls.nuspec -BasePath Package -Output Package