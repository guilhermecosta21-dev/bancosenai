const URL_API = 'https://localhost:7081/api/v1/Documento';

async function enviarDocumento() {
    const codigoCliente = document.getElementById("codigoCliente").value;
    const inputArquivo = document.getElementById("arquivo");
    const arquivo = inputArquivo.files[0];

    if (!codigoCliente || !arquivo) {
        alert("Informe o código do cliente e selecione um arquivo");
        return;
    }

    const dadosArquivo = new FormData();
    dadosArquivo.append("arquivo", arquivo);

    const response = await fetch(`${URL_API}/upload/${codigoCliente}`, {
        method: "POST",
        body: dadosArquivo
    });

    if (response.ok) {
        alert("Documento enviando com sucesso!");
        document.getElementById("codigoCliente").value = "";
        document.getElementById("arquivo").value = "";
    } else {
        const erro = await response.json();
        alert("Erro: " + (erro.message || "Falha ao enviar o documento"));
    }
}

async function buscarDocumentos() {
    const codigoCliente = document.getElementById("codigoClienteBusca").value;
    const tbody = document.querySelector("#tabelaDocumentos tbody");
    tbody.innerHTML = "";

    if (!codigoCliente) {
        alert("Informe o código do cliente para buscar.");
        return;
    }

    const response = await fetch(`${URL_API}/listar/${codigoCliente}`);
    
    if (response.ok) {
        const documentos = await response.json();
        documentos.forEach(doc => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${doc.id}</td>
                <td>${doc.name}</td>
                <td>${doc.extensao}</td>
                <td></td>
            `;
            tbody.appendChild(tr);
        });
    } else {
        alert("Nenhum documento encontrado para este cliente.");
    }
}