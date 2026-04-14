const formDiv = document.getElementById("formDiv");
const formLogin = document.getElementById("formLogin");
const formRegister = document.getElementById("formRegister");

let formState = 0;
let swapInProgress = 0;

function Entry() {
    UpdateRightTopBar();

    formLogin.classList.add("Hack");
    formLogin.addEventListener("animationend", LoginFadeCallback);
    formLogin.addEventListener("animationcancel", LoginFadeCallback);
    formRegister.addEventListener("animationend", RegisterFadeCallback);
    formRegister.addEventListener("animationcancel", RegisterFadeCallback);
}

// Igangsætter skift mellem de to formularer
function SwapForms() {
    if (swapInProgress) {
        return;
    }

    swapInProgress = 1;

    if (!formState) {
        formState = 1;
        
        if (formLogin.classList.contains("FadeIn")) {
            formLogin.classList.remove("FadeIn");
        }

        formLogin.classList.add("FadeOut");
    }
    else {
        formState = 0;

        if (formRegister.classList.contains("FadeIn")) {
            formRegister.classList.remove("FadeIn");
        }

        formRegister.classList.add("FadeOut");
    }
}

// Når loginformularen færdiggører en animation
function LoginFadeCallback() {
    if (formLogin.classList.contains("FadeOut")) {
        if (formLogin.classList.contains("Hack")) {
            formLogin.classList.remove("Hack");
        }

        formLogin.classList.add("Hidden");
        formLogin.classList.remove("FadeOut");
        
        formRegister.classList.remove("Hidden");
        formRegister.classList.add("FadeIn");
    }
    else if (formLogin.classList.contains("FadeIn")) {
        swapInProgress = 0;
    }
}

// Når registreringsformularen færdiggører en animation
function RegisterFadeCallback() {
    if (formRegister.classList.contains("FadeOut")) {
        formRegister.classList.add("Hidden");
        formRegister.classList.remove("FadeOut");

        formLogin.classList.remove("Hidden");
        formLogin.classList.add("FadeIn");
    }
    else if (formRegister.classList.contains("FadeIn")) {
        swapInProgress = 0;
    }
}

function TestLogin() {
    const loginUN = document.getElementById("loginUN");
    const loginPW = document.getElementById("loginPW");

    const un = sessionStorage.getItem("username");
    const pw = sessionStorage.getItem("password");

    if (loginUN.value === un && loginPW.value === pw) {
        window.open("/dashboard.html", "_self");
        sessionStorage.setItem("loggedin", "1");
    }
    else {
        alert("Forkert brugernavn eller adgangskode");
    }
}

function RegisterLoginCreds() {
    const regUN = document.getElementById("regUN");
    const regPW = document.getElementById("regPW");
    const regFirstName = document.getElementById("regNameF");
    const regLastName = document.getElementById("regNameL");

    sessionStorage.setItem("username", regUN.value);
    sessionStorage.setItem("password", regPW.value);

    sessionStorage.setItem("firstname", regFirstName.value);
    sessionStorage.setItem("lastname", regLastName.value);

    alert("Bruger oprettet!");
    SwapForms();
}

window.onload = Entry;
