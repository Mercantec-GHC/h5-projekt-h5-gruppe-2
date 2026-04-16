function IsLoggedIn() {
    return sessionStorage.getItem("loggedin");
}

function UpdateRightTopBar() {
    const userPivot = document.getElementById("userPivot");

    if (userPivot.children.length !== 0) {
        userPivot.removeChild(userPivot.firstChild);
    }

    const status = IsLoggedIn();
    if (status === null || status === "0") {
        const loginbutton = document.createElement("button");
        loginbutton.innerText = "Log ind";
        loginbutton.setAttribute("onclick", "GotoLoginPage()");

        userPivot.appendChild(loginbutton);
    }
    else {
        const userbutton = document.createElement("button");
        const fn = sessionStorage.getItem("firstname");
        const ln = sessionStorage.getItem("lastname");
        userbutton.innerText = fn + " " + ln;

        userPivot.appendChild(userbutton);
    }
}

function GotoLoginPage() {
    window.open("/login.html", "_self");
}

// Kaldes ingen steder fra, og er pt kun til test
function LogOut() {
    sessionStorage.setItem("loggedin", "0");
}
