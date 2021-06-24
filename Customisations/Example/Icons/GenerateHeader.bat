@echo off
REM Adrian Harwood, Version 1.0
REM The first two arguments specify the base width/height and the second 1.5 times the width/height.
REM This script assumes a default output path of .
REM To change the output path, supply it as the third command line argument.
REM To change the inkscape path, supply it as the fourth command line argument after the output path.

REM Create output directories
if "%1"=="" (
	set /A width=247
	set /A height=48
) else (
	set /A width=%1
	set /A height=%1
)
if "%2"=="" (
	if NOT "%1"=="" (
	echo Need to specify both base size and 1.5 * base size arguments!
	exit
)
	set /A width_h=371
	set /A height_h=72
) else (
	set /A width_h=%2
	set /A height_h=%2
)
if "%3"=="" (
	set out_dir=.
) else (
	set out_dir=%3
	if not exist %out_dir%\ (mkdir %out_dir%)
)
if "%4"=="" (
	set path_to_inkscape_exe="C:\Program Files\Inkscape\bin\inkscape.exe"
) else (
	set path_to_inkscape_exe=%4
)
mkdir %out_dir%\Android
mkdir %out_dir%\Android\drawable-mdpi
mkdir %out_dir%\Android\drawable-hdpi
mkdir %out_dir%\Android\drawable-xhdpi
mkdir %out_dir%\Android\drawable-xxhdpi
mkdir %out_dir%\Android\drawable-xxxhdpi
mkdir %out_dir%\iOS
mkdir %out_dir%\iOS\Header

REM Calculate sizes
set /A width_x=%width%*2
set /A height_x=%height%*2

set /A width_xx=%width%*3
set /A height_xx=%height%*3

set /A width_xxx=%width%*4
set /A height_xxx=%height%*4

REM Export
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\drawable-mdpi\ic_header_white.png" -w %width% -h %height% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\drawable-hdpi\ic_header_white.png" -w %width_h% -h %height_h% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\drawable-xhdpi\ic_header_white.png" -w %width_x% -h %height_x% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\drawable-xxhdpi\ic_header_white.png" -w %width_xx% -h %height_xx% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\drawable-xxxhdpi\ic_header_white.png" -w %width_xxx% -h %height_xxx% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Header\ic_header_white@1x.png" -w %width% -h %height% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Header\ic_header_white@2x.png" -w %width_x% -h %height_x% ic_header_white.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Header\ic_header_white@3x.png" -w %width_xx% -h %height_xx% ic_header_white.svg

