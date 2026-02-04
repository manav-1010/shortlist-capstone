document.addEventListener("click", function (e) {
    if (e.target.classList.contains("explain-btn")) {
        const card = e.target.closest(".card");
        const panel = card.querySelector(".explain-panel");
        panel.style.display = (panel.style.display === "none") ? "block" : "none";
    }
});

// priorities chips:
// - we store selected priorities as hidden inputs so MVC binds to List<string> Priorities
// - block selecting a 4th priority (max 3)
// - reset just clears the selected chips + hidden inputs

(function () {
    const chipsWrap = document.getElementById("priorityChips");
    const hiddenWrap = document.getElementById("priorityHiddenInputs");
    const errorEl = document.getElementById("priorityError");
    const countEl = document.getElementById("priorityCount");
    const resetBtn = document.getElementById("btnResetPriorities");

    if (!chipsWrap || !hiddenWrap) return;

    function getSelectedButtons() {
        return Array.from(chipsWrap.querySelectorAll(".priority-chip.selected"));
    }

    function getSelectedValues() {
        return getSelectedButtons().map(b => b.getAttribute("data-value"));
    }

    function rebuildHiddenInputs() {
        const selected = getSelectedValues();
        hiddenWrap.innerHTML = "";

        selected.forEach(v => {
            const input = document.createElement("input");
            input.type = "hidden";
            input.name = "Priorities";
            input.value = v;
            hiddenWrap.appendChild(input);
        });
    }

    function showError(show) {
        if (!errorEl) return;
        errorEl.style.display = show ? "block" : "none";
    }

    function updateCount() {
        if (!countEl) return;
        const n = getSelectedValues().length;
        countEl.textContent = `${n}/3`;
    }

    function resetPriorities() {
        getSelectedButtons().forEach(btn => {
            btn.classList.remove("selected");
            btn.setAttribute("aria-pressed", "false");
        });
        showError(false);
        rebuildHiddenInputs();
        updateCount();
    }

    // init
    rebuildHiddenInputs();
    updateCount();

    chipsWrap.addEventListener("click", function (e) {
        const btn = e.target.closest(".priority-chip");
        if (!btn) return;

        const isSelected = btn.classList.contains("selected");
        const selectedNow = getSelectedValues();

        // block 4th selection
        if (!isSelected && selectedNow.length >= 3) {
            showError(true);
            return;
        }

        btn.classList.toggle("selected");
        btn.setAttribute("aria-pressed", btn.classList.contains("selected") ? "true" : "false");

        showError(false);
        rebuildHiddenInputs();
        updateCount();
    });

    if (resetBtn) {
        resetBtn.addEventListener("click", resetPriorities);
    }
})();
