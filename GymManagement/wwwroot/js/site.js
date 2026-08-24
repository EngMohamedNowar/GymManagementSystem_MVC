document.addEventListener("DOMContentLoaded", function () {

    const themeToggle = document.getElementById("themeToggle");
    const themeIcon = document.getElementById("themeIcon");

    if (!themeToggle || !themeIcon) {
        return;
    }


    // Load saved theme
    const savedTheme = localStorage.getItem("gym-theme");


    if (savedTheme === "dark") {

        document.body.classList.add("dark-mode");

        themeIcon.classList.remove("bi-moon-fill");

        themeIcon.classList.add("bi-sun-fill");
    }


    // Toggle theme
    themeToggle.addEventListener("click", function () {

        document.body.classList.toggle("dark-mode");

        const isDark =
            document.body.classList.contains("dark-mode");


        // Save theme
        localStorage.setItem(
            "gym-theme",
            isDark ? "dark" : "light"
        );


        // Change icon
        if (isDark) {

            themeIcon.classList.remove("bi-moon-fill");

            themeIcon.classList.add("bi-sun-fill");

            themeToggle.title = "Light Mode";

        }
        else {

            themeIcon.classList.remove("bi-sun-fill");

            themeIcon.classList.add("bi-moon-fill");

            themeToggle.title = "Dark Mode";
        }

    });

});