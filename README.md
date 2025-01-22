# GFS in .NET8

### AVOLOAVOLO.it TRIBUTE

Given a list of airport codes as departure flights option, a list of airport codes as destinations and the search date range, this simple script will scrape all combination one by one and save the information to a CSV file about the first flight sorted.

Search parameters are configured in appSettings.json file.

## appSettings.json 

```json
{
  "FromAirports": [ "NAP" ],
  "ToAirports": [ "VIE", "IST" ],
  "FirstDepartureDate": "2023-12-01",
  "LastDepartureDate": "2023-12-11",
  "HowManyDays": 3,
  "FlexDays": 2,
  "OnlyWeekend": false
}
```

- FromAirports: **(list)** Departure airport codes;
- ToAirports: **(list)** Destination airport codes;
- FirstDepartureDate: **(string)** First available date for departure flight;
- LastDepartureDate: **(string)** Last available date for departure flight; 
- HowManyDays: **(integer)** The number of days between departure and return flights;
- FlexDays: **(integer)** Adds flexible days on the return flight date;
- OnlyWeekend: **(boolean)** The departure date is only on weekend days.

## Output

```cmd
Writing results in: 20231029223149.csv
ScraperLoop start.
2023-12-01 | 2023-12-04 | NAP-VIE | 219 € | Austrian | Diretto | 1 h 40 min
2023-12-01 | 2023-12-05 | NAP-VIE | 174 € | Biglietti separati prenotati insieme | 1 scalo | 4 h
2023-12-01 | 2023-12-06 | NAP-VIE | 156 € | Lufthansa, Austrian | 1 scalo | 4 h 25 min
2023-12-02 | 2023-12-05 | NAP-VIE | 133 € | Biglietti separati prenotati insieme | Diretto | 1 h 45 min
2023-12-02 | 2023-12-06 | NAP-VIE | 182 € | LufthansaAustrian | 1 scalo | 5 h 30 min
2023-12-02 | 2023-12-07 | NAP-VIE | 187 € | Lufthansa | 1 scalo | 5 h 30 min
2023-12-03 | 2023-12-06 | NAP-VIE | 156 € | Lufthansa, Austrian | 1 scalo | 4 h 25 min
2023-12-03 | 2023-12-07 | NAP-VIE | 161 € | Lufthansa, Austrian | 1 scalo | 4 h 25 min
2023-12-03 | 2023-12-08 | NAP-VIE | 161 € | Lufthansa, Austrian | 1 scalo | 4 h 25 min
2023-12-04 | 2023-12-07 | NAP-VIE | 155 € | Austrian | Diretto | 1 h 40 min
2023-12-04 | 2023-12-08 | NAP-VIE | 147 € | RyanairOperato da Lauda Europe | Diretto | 1 h 45 min
2023-12-04 | 2023-12-09 | NAP-VIE | 171 € | RyanairOperato da Lauda Europe | Diretto | 1 h 45 min
2023-12-05 | 2023-12-08 | NAP-VIE | 116 € | Wizz Air | Diretto | 1 h 45 min
2023-12-05 | 2023-12-09 | NAP-VIE | 138 € | Biglietti separati prenotati insieme | Diretto | 1 h 45 min
...
```

Other scraper settings such as Xpaths, Urls, etc. are in the other json files.
