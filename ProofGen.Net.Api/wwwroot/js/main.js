const resultado = document.getElementById("resultado");
let lastTicketData = null;

document.getElementById("uploadForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const formData = new FormData();
    const fileInput = document.querySelector('input[name="archivo"]');
    formData.append("archivo", fileInput.files[0]);

    const procesandoMsg = document.createElement('p');
    procesandoMsg.textContent = "Procesando nuevo ticket...";
    procesandoMsg.className = "text-gray-500 mb-2";
    resultado.appendChild(procesandoMsg);

    try {
        const response = await fetch("/api/tickets/extraer", {
            method: "POST",
            body: formData
        });

        if (!response.ok) throw new Error(`Código ${response.status}`);

        const data = await response.json();
        lastTicketData = data; // storing the data permanently.
        procesandoMsg.remove();
        resultado.insertAdjacentHTML('beforeend', renderTicket(data));
    } catch (error) {
        procesandoMsg.textContent = "Error al procesar: " + error.message;
        procesandoMsg.className = "text-red-600 mb-2";
    }
});

document.getElementById("clearTickets").addEventListener("click", () => {
    resultado.innerHTML = "";
});

function renderTicket(data) {
    return `
    <div class="bg-white p-3 text-[12px] font-mono leading-normal text-gray-900 border border-gray-300 rounded w-[350px] mx-auto shadow">
        <div class="text-center mb-3">
            <p class="font-bold text-[14px]">TICKET PARA FACTURACION</p>
        </div>
        <hr class="border-dashed border-t border-gray-400 my-3" />
        <div class="text-center mb-3">
            <p class="font-bold text-[14px]">${data.legalName}</p>
        </div>
        <hr class="border-dashed border-t border-gray-400 my-3" />
        <div class="mb-4">
            <p><strong>RFC:</strong> ${data.federalTaxpayerRegistry}</p>
            <p><strong>Fecha:</strong> ${new Date(data.date).toLocaleDateString()}</p>
            <p><strong>Hora:</strong> ${data.hours}</p>
            <p><strong>Caja:</strong> ${data.checkOut}</p>
            <p><strong>Cajero:</strong> ${data.cashier}</p>
        </div>
        <hr class="border-dashed border-t border-gray-400 my-3" />
        ${data.products.map(p => `
            <div class="flex justify-between mb-1">
                <span class="break-words">${p.quantity} x ${p.description}</span>
                <span>$${p.ammount.toFixed(2)}</span>
            </div>
        `).join("")}
        <hr class="border-dashed border-t border-gray-400 my-3" />
        <div class="text-right mb-4">
            <p><strong>Total:</strong> $${data.totalAmount.toFixed(2)}</p>
            <p><strong>Tarjeta:</strong> $${data.card.toFixed(2)}</p>
            <p><strong>Cambio:</strong> $${data.change.toFixed(2)}</p>
        </div>
        <hr class="border-dashed border-t border-gray-400 my-3" />
        <div class="text-center text-[11px] text-gray-600 leading-tight whitespace-pre-wrap break-words">
            ${data.address.replace(/\n/g, "<br>")}
        </div>
    </div>
    `;
}

document.getElementById("downloadPdf").addEventListener("click", async function () {
    if (!lastTicketData) {
        alert("No hay ticket para descargar.");
        return;
    }

    try {
        const response = await fetch("/api/ticket/pdf", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(lastTicketData)
        });

        if (!response.ok) throw new Error("Error al generar el PDF.");

        const blob = await response.blob();

        // Crear un enlace para descargar
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = "ticket.pdf";
        link.click();
        URL.revokeObjectURL(link.href);
    } catch (err) {
        alert("Error al descargar el PDF: " + err.message);
    }
});


document.getElementById("archivo").addEventListener("change", function () {
    const nombreArchivo = this.files[0] ? this.files[0].name : "Ningún archivo seleccionado";
    document.getElementById("archivoSeleccionado").textContent = nombreArchivo;
});