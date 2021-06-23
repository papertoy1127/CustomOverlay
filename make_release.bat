
SETLOCAL ENABLEDELAYEDEXPANSION
set /p version=<VERSION.txt
mkdir tmp
cd tmp
mkdir CustomOverlay
cp ../Info.json CustomOverlay
cp ../CustomOverlay/bin/Release/CustomOverlay.dll CustomOverlay

cd CustomOverlay
for /f "delims=" %%a in (Info.json) do (
    SET s=%%a
    SET s=!s:$VERSION=%version%!
    echo !s!
) >>"InfoChanged.json"
rm Info.json
mv InfoChanged.json Info.json
cd ..

tar -a -c -f CustomOverlay-%version%.zip CustomOverlay
mv CustomOverlay-%version%.zip ..
cd ..
rm -rf tmp
pause