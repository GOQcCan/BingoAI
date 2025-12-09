@echo off
REM Script pour installer dotnet-ef et créer la migration initiale

echo Installation de dotnet-ef...
dotnet tool install --global dotnet-ef

echo.
echo Attente de 5 secondes pour permettre la mise à jour du PATH...
timeout /t 5 /nobreak

echo.
echo Navigation vers le dossier du projet...
cd /d "%~dp0BingoAI.Server"

echo.
echo Création de la migration InitialCreateWithGuid...
dotnet ef migrations add InitialCreateWithGuid

echo.
echo Migration créée avec succès!
echo.
echo Pour appliquer la migration, exécutez:
echo   dotnet ef database update
echo.
echo Ou lancez simplement l'application, elle sera appliquée automatiquement.
echo.
pause
