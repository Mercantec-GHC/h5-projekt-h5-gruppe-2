const Chart = window.Chart;

class DataEntry {
    Time;
    Intensity;

    constructor(time, intensity) {
        this.Time = time;
        this.Intensity = intensity;
    }
}

/*  ======  NOTER  ======

    Design:
    Giver måske mest mening at dele tingene op i nogle kolonner, sådan som jeg har gjort, med 2 små på siden til menu, og 1 stor i midten til data.

    Data:
    Det bliver en udfordring at både opbevare og vise data hos klienten, hvis der bliver sendt for mange punkter.
    Det ville give mening at dele det i at få data fra fx de sidste 24 timer, sidste 3 dage, sidste uge, osv?
    Der har vi dog et problem. Der er 60 * 60 * 24 = 86400 sekunder på et døgn, og vi måler 2x i sekundet -
      og det giver 172800 nye tal i databasen hver dag, per enhed, hvis der måles i 24 timer.
    Det giver simpelthen ikke mening at vise brugeren over 100.000 tal, og det giver en forfærdeligt kompliceret graf (hvis den overhovedet kan dannes),
      plus tabellen bliver overfyldt og uoverskuelig.
    Måske skal mængden, eller rettere densiteten, af data vi sender sænkes. Måske give et loft på en N mængde tal per interval eller sætte faste mellemrum per interval.
    I det andet tilfælde, kunne det fx resultere i disse intervaller: 
      - 1 time:         Hvert 15. sekund    (240 tal)
      - 6 timer:        Hvert minut         (360 tal)
      - 24 timer:       Hvert 5. minut      (288 tal)
      - 3 dage:         Hvert 15. minut     (288 tal)
      - 1 uge:          Hver halve time     (336 tal)

    Med den reducerede mængde data, kan vi mindske læsset på både server og klient, og også give mulighed for at se "finere" data ved ønske.
    Jeg kan ikke komme i tanke om en situation, hvor der er brug for adgang til data per sekund hos brugeren (nu måler vi også forfærdeligt ofte som en del af H5-kravene).
*/



/*
 id |                      data                      
----+------------------------------------------------
  1 | {"value": 90, "timestamp": 1774877411.0069919}
  2 | {"value": 90, "timestamp": 1774877411.5064216}
  3 | {"value": 90, "timestamp": 1774877412.0063818}
  4 | {"value": 90, "timestamp": 1774877412.509814}
  5 | {"value": 91, "timestamp": 1774877413.0097523}
  6 | {"value": 91, "timestamp": 1774877413.5091648}
  7 | {"value": 91, "timestamp": 1774877414.0091364}
  8 | {"value": 90, "timestamp": 1774877414.5086844}
  9 | {"value": 91, "timestamp": 1774877415.0086913}
 10 | {"value": 90, "timestamp": 1774877415.508071}
 11 | {"value": 91, "timestamp": 1774877416.011905}
 12 | {"value": 91, "timestamp": 1774877416.5114796}
 13 | {"value": 91, "timestamp": 1774877417.0114346}
 14 | {"value": 91, "timestamp": 1774877417.5108135}
 15 | {"value": 91, "timestamp": 1774877418.0103397}
 16 | {"value": 91, "timestamp": 1774877418.5103743}
 17 | {"value": 91, "timestamp": 1774877419.0142257}
 18 | {"value": 92, "timestamp": 1774877419.513587}
 19 | {"value": 91, "timestamp": 1774877420.013152}
 20 | {"value": 92, "timestamp": 1774877420.513105}
 21 | {"value": 92, "timestamp": 1774877421.0130775}
 22 | {"value": 91, "timestamp": 1774877421.512522}
 23 | {"value": 91, "timestamp": 1774877422.0120492}
 24 | {"value": 92, "timestamp": 1774877422.515904}
 25 | {"value": 91, "timestamp": 1774877423.015849}
 26 | {"value": 90, "timestamp": 1774877423.5152543}
 27 | {"value": 91, "timestamp": 1774877424.014802}
 28 | {"value": 91, "timestamp": 1774877424.5147579}
*/

const testDataString = [
    "1774857744.9164872,100",
    "1774857745.4164336,109",
    "1774857745.9160151,118",
    "1774857746.4159825,128",
    "1774857746.9153712,137",
    "1774857747.4149559,144",
    "1774857747.9149501,152",
    "1774857748.4188035,159",
    "1774857748.9181638,167",
    "1774857749.4176912,171",
    "1774857749.917646,171",
];

const testDataJSON = [
    {
        time: "1774857744.9164872",
        light: "100"
    },
    {
        time: "1774857745.4164336",
        light: "109"
    },
    {
        time: "1774857745.9160151",
        light: "118"
    },
    {
        time: "1774857746.4159825",
        light: "128"
    },
    {
        time: "1774857746.9153712",
        light: "137"
    },
    {
        time: "1774857747.4149559",
        light: "144"
    },
    {
        time: "1774857747.9149501",
        light: "152"
    },
    {
        time: "1774857748.4188035",
        light: "159"
    },
    {
        time: "1774857748.9181638",
        light: "167"
    },
    {
        time: "1774857749.4176912",
        light: "171"
    },
    {
        time: "1774857749.917646",
        light: "171"
    },
];

const dataSnippet = [
    {
        "id": 1,
        "data": {
            "value": 90,
            "timestamp": 1774877411.0069919
        }
    },
    {
        "id": 2,
        "data": {
            "value": 90, 
            "timestamp": 1774877411.5064216
        }
    },
    {
        "id": 3,
        "data": {
            "value": 90, 
            "timestamp": 1774877412.0063818
        }
    },
    {
        "id": 4,
        "data": {
            "value": 90, 
            "timestamp": 1774877412.509814
        }
    },
    {
        "id": 5,
        "data": {
            "value": 91, 
            "timestamp": 1774877413.0097523
        }
    },
    {
        "id": 6,
        "data": {
            "value": 91, 
            "timestamp": 1774877413.5091648
        }
    },
    {
        "id": 7,
        "data": {
            "value": 91, 
            "timestamp": 1774877414.0091364
        }
    },
    {
        "id": 8,
        "data": {
            "value": 90, 
            "timestamp": 1774877414.5086844
        }
    },
    {
        "id": 9,
        "data": {
            "value": 91, 
            "timestamp": 1774877415.0086913
        }
    },
    {
        "id": 10,
        "data": {
            "value": 90, 
            "timestamp": 1774877415.508071
        }
    },
    {
        "id": 11,
        "data": {
            "value": 91, 
            "timestamp": 1774877416.011905
        }
    },
    {
        "id": 12,
        "data": {
            "value": 91, 
            "timestamp": 1774877416.5114796
        }
    },
    {
        "id": 13,
        "data": {
            "value": 91, 
            "timestamp": 1774877417.0114346
        }
    },
    {
        "id": 14,
        "data": {
            "value": 91, 
            "timestamp": 1774877417.5108135
        }
    },
    {
        "id": 15,
        "data": {
            "value": 91, 
            "timestamp": 1774877418.0103397
        }
    },
    {
        "id": 16,
        "data": {
            "value": 91, 
            "timestamp": 1774877418.5103743
        }
    },
    {
        "id": 17,
        "data": {
            "value": 91, 
            "timestamp": 1774877419.0142257
        }
    },
    {
        "id": 18,
        "data": {
            "value": 92, 
            "timestamp": 1774877419.513587
        }
    },
    {
        "id": 19,
        "data": {
            "value": 91, 
            "timestamp": 1774877420.013152
        }
    },
    {
        "id": 20,
        "data": {
            "value": 92, 
            "timestamp": 1774877420.513105
        }
    },
    {
        "id": 21,
        "data": {
            "value": 92, 
            "timestamp": 1774877421.0130775
        }
    },
    {
        "id": 22,
        "data": {
            "value": 91, 
            "timestamp": 1774877421.512522
        }
    },
    {
        "id": 23,
        "data": {
            "value": 91, 
            "timestamp": 1774877422.0120492
        }
    },
    {
        "id": 24,
        "data": {
            "value": 92, 
            "timestamp": 1774877422.515904
        }
    },
    {
        "id": 25,
        "data": {
            "value": 91, 
            "timestamp": 1774877423.015849
        }
    },
    {
        "id": 26,
        "data": {
            "value": 90, 
            "timestamp": 1774877423.5152543
        }
    },
    {
        "id": 27,
        "data": {
            "value": 91, 
            "timestamp": 1774877424.014802
        }
    },
    {
        "id": 28,
        "data": {
            "value": 91, 
            "timestamp": 1774877424.5147579
        }
    },
];

// Bruger rene strings
function ParseDataString(dataString) {
    const splitIndex = dataString.indexOf(",");

    if (splitIndex === -1) {
        return -1;
    }

    const timeDirtyString = dataString.substring(0, splitIndex);
    let timeString;

    const timeSplitIndex = timeDirtyString.indexOf(".");

    if (timeSplitIndex !== -1) {
        const seconds = timeDirtyString.substring(0, timeSplitIndex);
        const milliseconds = timeDirtyString.substring(timeSplitIndex + 1, timeSplitIndex + 4);
        timeString = String(seconds) + String(milliseconds);
    }

    const intensity = dataString.substring(splitIndex + 1);

    const temp = Temporal.Instant.fromEpochMilliseconds(Number(timeString));
    const timeLocale = temp.toLocaleString();

    let data = new DataEntry(timeLocale, intensity);

    return data;
}

// Bruger data, som jeg har skrevet om til JSON format fra strings
function ParseDataJSON(dataJSON) {
    const timeDirty = dataJSON.time;
    const intensity = dataJSON.light;

    const timeSplitIndex = timeDirty.indexOf(".");
    let timeString;

    if (timeSplitIndex !== -1) {
        const seconds = timeDirty.substring(0, timeSplitIndex);
        const milliseconds = timeDirty.substring(timeSplitIndex + 1, timeSplitIndex + 4);
        timeString = String(seconds) + String(milliseconds);
    }
    else {
        timeString = "";
    }

    const temp = Temporal.Instant.fromEpochMilliseconds(Number(timeString));
    const timeLocale = temp.toLocaleString();

    let data = new DataEntry(timeLocale, intensity);

    return data;
}

// Bruger data i JSON fra serveren (indtil videre)
function ParseJSON(dataJSON) {
    //const id = dataJSON.id;
    const timeDirty = dataJSON.data.timestamp;
    const intensity = dataJSON.data.value;

    const time = Math.trunc(timeDirty * 1000);

    const temp = Temporal.Instant.fromEpochMilliseconds(time);
    const timeLocale = temp.toLocaleString();

    let data = new DataEntry(timeLocale, intensity);

    return data;
}

function SetUpGraph(data) {
    new Chart(
        document.getElementById("testGraph"), {
            type: "line",
            data: {
                labels: data.map(row => row.Time),
                datasets: [
                    {
                        label: "Lysstyrke",
                        data: data.map(row => row.Intensity)
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        }
    );
}

function SetUpTable() {
    const tableDiv = document.getElementById("testTableDiv");
    let dataArray = [];

    const table = document.createElement("table");
    table.className = "Table";
    table.id = "testTable";
    
    const hRow = document.createElement("tr");

    const hTime = document.createElement("th");
    hTime.innerText = "Tid";
    hRow.appendChild(hTime);
    
    const hData = document.createElement("th");
    hData.innerText = "Lysintensitet";
    hRow.appendChild(hData);

    table.appendChild(hRow);

    /*
    for (let i = 0; i < testDataJSON.length; i++) {
        //const string = testDataString[i];
        //const data = ParseDataString(string);

        const data = ParseDataJSON(testDataJSON[i]);
        dataArray.push(data);

        if (data === -1) {
            alert("fug");
        }
        
        const row = document.createElement("tr");

        const time = document.createElement("td");
        time.innerText = data.Time;
        time.className = "TableStat";
        row.appendChild(time);

        const intensity = document.createElement("td");
        intensity.innerText = data.Intensity;
        intensity.className = "TableStat";
        row.appendChild(intensity);

        table.appendChild(row);
    }
    */

    for (let i = 0; i < dataSnippet.length; i++) {
        //const string = testDataString[i];
        //const data = ParseDataString(string);

        const data = ParseJSON(dataSnippet[i]);
        dataArray.push(data);

        if (data === -1) {
            alert("fug");
        }
        
        const row = document.createElement("tr");

        const time = document.createElement("td");
        time.innerText = data.Time;
        row.appendChild(time);

        const intensity = document.createElement("td");
        intensity.innerText = data.Intensity;
        row.appendChild(intensity);

        table.appendChild(row);
    }

    // Skal flyttes senere
    SetUpGraph(dataArray);

    tableDiv.appendChild(table);
}

function NowPressed() {
    const temp = Temporal.Now.instant();
    const time = temp.toZonedDateTimeISO("Europe/Copenhagen");

    const dateInput = document.getElementById("dateInput");
    const timeInputHour = document.getElementById("timeInputHour");
    const timeInputMinute = document.getElementById("timeInputMinute");

    let dayString = "";
    if (10 > Number(time.day)) {
        dayString = "0";
    }
    dayString += String(time.day);

    let monthString = "";
    if (10 > Number(time.month)) {
        monthString = "0";
    }
    monthString += String(time.month);

    const dateString = String(time.year) + "-" + String(monthString) + "-" + String(dayString);

    dateInput.value = dateString;
    timeInputHour.value = String(time.hour);
    timeInputMinute.value = String(time.minute);
}

window.onload = SetUpTable;
