@echo off
REM Adrian Harwood, Version 1.0.1
REM This script assumes a default output path of ".".
REM To change the output path, supply it as the first command line argument.
REM This script assumes the default path to inkscape of "C:\Program Files\Inkscape".
REM To change the inkscape path, supply it as the second command line argument after the output path.
REM Inkscape can be downloaded here: https://inkscape.org/

REM Create output directories
if "%1"=="" (
	set out_dir=.
) else (
	set out_dir=%1
	if not exist %out_dir%\ (mkdir %out_dir%)
)
if "%2"=="" (
	set path_to_inkscape_exe="C:\Program Files\Inkscape\bin\inkscape.exe"
) else (
	set path_to_inkscape=%2
)
mkdir %out_dir%\Android
mkdir %out_dir%\Android\mipmap-mdpi
mkdir %out_dir%\Android\mipmap-hdpi
mkdir %out_dir%\Android\mipmap-xhdpi
mkdir %out_dir%\Android\mipmap-xxhdpi
mkdir %out_dir%\Android\mipmap-xxxhdpi
mkdir %out_dir%\iOS
mkdir %out_dir%\iOS\Icon

REM Android legacy icons
echo Android legacy...
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-mdpi\icon.png" -w 48 -h 48 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-hdpi\icon.png" -w 72 -h 72 icon_20px.svg 
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-xhdpi\icon.png" -w 96 -h 96 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-xxhdpi\icon.png" -w 144 -h 144 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-xxxhdpi\icon.png" -w 192 -h 192 icon_20px.svg

REM Android adaptable icons
echo Android adaptable...
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-mdpi\launcher_foreground.png" -w 108 -h 108 adaptive_108px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-hdpi\launcher_foreground.png" -w 162 -h 162 adaptive_108px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-xhdpi\launcher_foreground.png" -w 216 -h 216 adaptive_108px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-xxhdpi\launcher_foreground.png" -w 324 -h 324 adaptive_108px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\mipmap-xxxhdpi\launcher_foreground.png" -w 432 -h 432 adaptive_108px.svg

REM iOS icons
echo iOS icons...
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon20.png" -w 20 -h 20 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon29.png" -w 29 -h 29 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon40.png" -w 40 -h 40 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon58.png" -w 58 -h 58 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon60.png" -w 60 -h 60 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon76.png" -w 76 -h 76 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon80.png" -w 80 -h 80 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon87.png" -w 87 -h 87 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon120.png" -w 120 -h 120 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon152.png" -w 152 -h 152 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon167.png" -w 167 -h 167 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon180.png" -w 180 -h 180 icon_20px.svg
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\iOS\Icon\icon1024.png" -w 1024 -h 1024 icon_20px.svg

REM Google play icon
echo Google play...
%path_to_inkscape_exe% --export-type="png" --export-filename="%out_dir%\Android\icon_play.png" -w 512 -h 512 icon_20px.svg