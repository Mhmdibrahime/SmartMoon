const inputs = document.querySelectorAll(".input");

function addcl() {
    let parent = this.parentNode.parentNode;
    parent.classList.add("focus");
}

function remcl() {
    let parent = this.parentNode.parentNode;
    if (this.value == "") {
        parent.classList.remove("focus");
    }
}

inputs.forEach(input => {
    input.addEventListener("focus", addcl);
    input.addEventListener("blur", remcl);
});

function validateForm() {
    const username = document.getElementById("username");
    const password = document.getElementById("password");
    let valid = true;

    if (username.value.trim() === "") {
        username.parentElement.parentElement.classList.add("input-error");
        valid = false;
    } else {
        username.parentElement.parentElement.classList.remove("input-error");
    }

    if (password.value.trim() === "") {
        password.parentElement.parentElement.classList.add("input-error");
        valid = false;
    } else {
        password.parentElement.parentElement.classList.remove("input-error");
    }

    if (valid) {
        window.location.href = "الرئيسيه.html";
    }
    
    return false;
}
