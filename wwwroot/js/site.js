document.addEventListener("click", function (e) {
    if (e.target.classList.contains("explain-btn")) {
        const card = e.target.closest(".card");
        const panel = card.querySelector(".explain-panel");
        panel.style.display = (panel.style.display === "none") ? "block" : "none";
    }
});
