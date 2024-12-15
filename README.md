**Kontrola publicznego adresu IP**

**Instrukcja uruchomienia usługi**


> [!CAUTION]
> Obowiązkowo SDK i Runtime jeśli na linux! w wersji 8.0 lub nowszej.
> [Instalowanie platformy .NET w systemie Linux](https://learn.microsoft.com/pl-pl/dotnet/core/install/linux)

Jeśli masz już platformę .Net
Linux:

1. Najpierw utwórz plik:
AddressIPControlBackgroundService.service a w nim:

```
[Unit]
Description=Sprawdza publiczny adres IP i wysyla e-mail gdy sie zmieni

[Service]
WorkingDirectory=/home/vento87/Desktop/_PublicAddress
ExecStart=/home/vento87/Desktop/_PublicAddress/AddressIPControlBackgroundService
Restart=always
RestartSec=no
KillSignal=SIGINT
SyslogIdentifier=ventoapp-adress-ip-control
User=vento87
Environment=ASPNETCORE_ENVIRONMENT=Production
EnvironmentFile=/etc/environment

[Install]
WantedBy=multi-user.target
```
2. Instalacja usługi:
```
cd /home/vento87/Desktop/_PublicAddress                                         ## przechodzisz do katalogu z apką
chmod +x /home/vento87/Desktop/_PublicAddress/AddressIPControlBackgroundService ## Nadanie uprawnień aplikacji
sudo cp AddressIPControlBackgroundService.service /etc/systemd/system/          ## Kopiujesz AddressIPControlBackgroundService.service do katalogu startowego
export DOTNET_ROOT=$HOME/.dotnet                                                ## potrzebne :)
export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools                               ## to też potrzebne :)
sudo systemctl daemon-reload                                                    ## przeładowujesz daemon'a
sudo systemctl enable AddressIPControlBackgroundService                         ## ustawiasz apkę na tryb włączania z systemem
sudo systemctl start AddressIPControlBackgroundService                          ## startujesz usługę lub stop gdy chcesz zatrzymać
sudo systemctl status AddressIPControlBackgroundService                         ## sprawdzasz status czy nie ma błędów, apka ma rozbudowany system logów, będziesz wiedział, co kiedy robiła :)
```
3. Usuwanie usługi:
```
sudo systemctl disable AddressIPControlBackgroundService
sudo rm /etc/systemd/system/AddressIPControlBackgroundService.service
```

Windows:
Nie chce mi się pisać instrukcji ale działa :)
