  #Script is for updating the application 
  #
  #
  #
  #
  #Marcel Curtusan 
  #4/27/2023
  
    param (
      [Parameter(Mandatory=$true)]
    [string] $Version,
      [Parameter(Mandatory=$true)]
    [string] $AppName,
      [Parameter(Mandatory=$true)]
    [string] $DrivePath,
      [Parameter(Mandatory=$true)]
    [string] $UserName,
      [Parameter(Mandatory=$true)]
    [string] $Token
    )
    $env:PSModulePath
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls -bor
    [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12
    
    #Software location
    $SoftwareLocation = "Software Package\"
	[string] $DrivePathLocation = -join ($DrivePath, ":\")
    [string] $LogLocation = -join ($AppName,"\UpdateLogs\",$AppName,"Update.log")
    # Initialize logging
    [string] $LogPath = [IO.Path]::Combine([string[]]($DrivePathLocation ,$SoftwareLocation, $LogLocation)) 
    Start-Transcript -Path $LogPath -Append
        try {
             # Get the IIS WebAdministration module
            Import-Module WebAdministration
	
            Write-Host "Drive Path:" $DrivePathLocation
            Write-Host "Version:" $Version
            Write-Host "Web App:" $AppName
            #set drive letter
             # Specify the new physical path
            $newPhysicalPath = [IO.Path]::Combine([string[]]($DrivePathLocation ,$SoftwareLocation, $AppName, $Version ))
            Write-Host "Physical Path:" $newPhysicalPath
            # Check if the folder with the same version name already exists
            if (Test-Path $newPhysicalPath) {
                #Write-Host "Removed the folder with the same version name"
                Write-Host "Unable to continue $Version Version already exist Install will not continue "
                return
            }
            $lowerAppName = $AppName.ToLower()
		    $zipUrl = "https://artifactory.usps.gov/artifactory/eir-9209-release/gov/usps/eir9209/$lowerAppName/$Version/$Version.zip"
		
            Write-Host "URL:" $zipUrl
            
            # Check if the zip file already exists
            $zipPath = [IO.Path]::Combine([string[]]($DrivePathLocation,$SoftwareLocation, $AppName, "$Version.zip"))
            Write-Host "ZIP Path:" $zipPath
			if (Test-Path $zipPath) {
                Write-Host "Zip file already exists, deleting ..."
                Remove-Item $zipPath
                Write-Host "Zip file Removed"
            }
            $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $username,$token)))
            # Download file
            Invoke-RestMethod -Uri $zipUrl -Method Get -ContentType "application/zip" -Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} -OutFile $zipPath

            # Extract the contents of the zip file to the new physical path
            Expand-Archive -LiteralPath $zipPath -DestinationPath $newPhysicalPath -Force

            # Check if the new physical path exists
            if (-not(Test-Path $newPhysicalPath)) {
                 Write-Error "Error: The specified physical path does not exist"
                 return
            }
            # Check if the application pool already exists
            if (Get-Item "IIS:\AppPools\$AppName" -ErrorAction SilentlyContinue) {
                Write-Host "Application pool '$AppName' already exists."
            } else {
                # Create the new application pool
                New-Item "IIS:\AppPools\$AppName"
                Write-Host "Application pool '$AppName' created."
            }

            # Check if the application already exists in the default website
            if (Get-Item "IIS:\Sites\Default Web Site\$AppName" -ErrorAction SilentlyContinue) {
                Write-Host "Application '$AppName' already exists in the default website."
            } else {
                # Create the new application in the default website
                New-Item "IIS:\Sites\Default Web Site\$AppName" -type Application -physicalPath $newPhysicalPath -applicationPool $AppName
                Write-Host "Application '$AppName' added to the default website."

            }
            # Get the web application object
            $webApp = Get-WebApplication -Site "Default Web Site" -Name $AppName -ErrorAction Stop

            # Stop the application pool associated with the web application
            $appPoolName = $webApp.applicationPool
            $appPool = Get-WebAppPoolState -Name $appPoolName -ErrorAction Stop
            if ($appPool.value -eq "Started") {
               # Stop the application pool associated with the web application
                Stop-WebAppPool -Name $appPoolName -ErrorAction Stop
                Write-Host "AppPool is " $appPool.value
               # Check if the application pool has stopped, with a timeout of 60 seconds
                    $timeout = 60
                    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
                    do {
                        $appPool = Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue
                        if ($appPool.value -eq "Stopped") {
                            break
                        }
                        Start-Sleep -Seconds 1
                    } while ($stopwatch.Elapsed.Seconds -lt $timeout)
                }
            if ($appPool.value -ne "Stopped") {
                throw "Error: Timeout waiting for application pool to stop"
            }
           
              # Update the physical path of the web application

              $strIISPath = "IIS:\Sites\Default Web Site\" + $AppName
            Set-ItemProperty -Path $strIISPath -Name "physicalPath" -Value $newPhysicalPath -ErrorAction Stop


          # Start the application pool associated with the web application, only if it was previously running
            if ($appPool.value -eq "Stopped") {
        
                Start-WebAppPool -Name $appPoolName -ErrorAction Stop
                Write-Host "AppPool is " $appPool.value
            }

            Write-Output "Physical path updated successfully and application pool restarted"
        }
        catch {
            Write-Error "Error: $($_.Exception.Message)"
        }
        finally {
            # Stop logging
            Stop-Transcript
        }
