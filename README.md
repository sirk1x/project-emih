# project-emih
Emih is a Discord bot that features support for the OpenAI API of ChatGPT &amp; DALL·E image generation. Can be used on unix & windows systems.


## Features

- /auth - to authenticate the given server by an admin.
- /think - to access ChatGPT and ask questions.
- /vision - to generate images by DALL-E.


## Getting Started
### Windows
Simply download the release package and extract the contents somewhere. Continue with the configuration section.
### Unix
To check if your linux installation supports .net 7.0 check https://learn.microsoft.com/en-us/dotnet/core/install/linux

## Debian 11 Installation
Download the dependencies needed and unzip the bot to /opt/emih/
```sh
adduser emih --shell=/bin/false 
wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
apt-get update
apt-get install -y dotnet-sdk-7.0 unzip
mkdir /opt/emih
wget -O /opt/emih/emih.zip https://github.com/sirk1x/project-emih/releases/download/release/emih.zip
unzip /opt/emih/emih.zip -d /opt/emih/
rm /opt/emih/emih.zip
chown -R emih:emih /opt/emih
```
Create Systemd service.
```sh
tee /etc/systemd/system/emih.service <<EOF
[Unit]
Description=Emih
After=multi-user.target
After=network-online.target
Wants=network-online.target

[Service]
ExecStart=dotnet /opt/emih/project-emih.dll
User=emih
Group=emih
Type=idle
Restart=always
RestartSec=15
RestartPreventExitStatus=0
TimeoutStopSec=10

[Install]
WantedBy=multi-user.target
EOF
```
Enable the service and start the bot.
```sh
systemctl daemon-reload
systemctl enable emih
systemctl start emih

```
### Configuration
Emih will create configuration files on the first run. 
Simply edit the ```config.json``` inside emihs folder and add the [Discord](https://discord.com/developers/applications) app id, bot token and the [OpenAI API](https://platform.openai.com/account/api-keys) key. 
Configure other parameters as needed.
```sh
{
  "Dalle_Images_Per_Request": 1,
  "Dalle_Image_Size": "1024x1024",
  "BotStatus": 1,
  "BotGame": "",
  "BotName": "Emih",
  "OpenAI_Api_Key": "OPEN AI API KEY HERE",
  "Discord_Bot_Token": "DISCORD BOT TOKEN HERE",
  "Discord_App_Id": Application ID
}
```
Restart the bot to apply the changes. 
```
systemctl restart emih
```
Inside the ```invite.txt``` you can find the invitation link for the bot.
```sh
cat /opt/emih/invite.txt
```
Upon inviting emih to a discord server, run the command ```/auth``` somehwere in the server. 
If you aren't authenticated as a admin yet, the bot will display your user id. 
Copy the user id and replace the 0 inside the admins.json or add more admins if needed.
```sh
[
  YOUR_USER_ID,
  ANOTHER_USER_ID,
  ...
]
```
Restart the bot to apply the changes. 
```
systemctl restart emih
```
Run the ```/auth``` command and the server should be authenticated!

## Discord Bot Settings
To claim the Bot Token needed for emih, navigate to the [discord developer portal](https://discord.com/developers/applications) and create a new application. 
Select bot on the left and hit ```reset token``` and copy the bot token.
![alt text](https://www.traumabande.de/github/token.png "Default Text")

The following intents should be marked as active.
![alt text](https://www.traumabande.de/github/intents.png "Default Text")

## License

Licensed under the GNU Affero General Public License v3.0

**Free Software, Hell Yeah!**
