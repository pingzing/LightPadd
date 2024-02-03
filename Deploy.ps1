# Build and deploy the app to the Raspberry Pi

# BIG 'OL TODO:

# make sure our cwd is the repo root
# Publish via 'dotnet publish -c Release -r linux-arm64 --self-contained true -p:IncludeNativeLibrariesForSelfExtract=true'

# Output will be in .\LightPadd.Core\bin\Release\net7.0\linux-arm64\publish\.

# ssh to do 'systemctl stop lightpadd.service'

# scp over the new binary by doing:
#   'scp .\LightPadd.Core\bin\Release\net7.0\linux-arm64\publish\LightPadd.Core padd@192.168.0.43:/home/padd/lightPadd'

# ssh to do 'systemctl start lightpadd.service'

# if we're feeling extra-fancy, send a request to the various Hubitat APIs to redirect the POST URL to the Pi's IP Address