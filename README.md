# Exchange Rates
Downloading exchange rates against the ruble and converting them among themselves.

# Fast start
**You can start services from project root folder from command line with command:**

_docker compose up_


**When project will be started you can call Get-methods:**

http://localhost/api/Currency/UpdateCurrencies - downloading curencies

http://localhost/api/Currency/GetCurrencies - return currencies from DB

# Docker compose contains
RabbitMq, Postgre, Crawler, Storage, Converter

# About
Downloading info about currencies from: "http://cbr.ru/scripts/XML_valFull.asp"

Downloading info about rates from: "http://cbr.ru/scripts/XML_daily.asp?date_req="

**I'm using Saga MassTransit with Request which has event fro start saga UpdateCurrency and steps:**

1)RequestCurrencyRates - downloading info from http://cbr.ru (Crawler service)

2)UpdateCurrencyInfo - saving info about currencies in DB (Storage service)

3)ConverRate -  converting currencies among themselves (Converter service)

4)SaveRates - saving rates in DB (Storage service)
