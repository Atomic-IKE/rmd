# rmd

## Summary

```rmd``` searches for duplicate files in a folder (including subfolders recursively, if ```-r``` option is specified) and moves them to another folder (```bin```), specified with ```-b``` option. Only the first copy found is kept, copies found later are moving to ```bin```.  

## How to use it?

```
rmd.exe -d "C:\possible-duplicates" -r -b "c:\duplicates"
```

```possible-duplicates``` is the folder with possibly duplicate files. 

```-r``` option includes subfolders to search path. 

```-b``` option followed by path is used to specify a folder where duplicate files will be moved to. 
