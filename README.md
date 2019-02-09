To deploy:
First, run dotnet publish -r linux-arm to deploy for linux based ARM machines, like the Raspberry Pi.
Then transfer that over to the pi:
scp -r publish pi@192.168.178.29:~/Documents/rpi

After that you're good to go.