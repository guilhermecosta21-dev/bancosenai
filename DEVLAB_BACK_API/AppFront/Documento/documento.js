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
    }
    else {
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
                <td>
                    <button class="btn-baixar" onclick="baixarArquivo(${doc.id})">Baixar</button>
                    <button class="btn-excluir" onclick="excluirArquivo(${doc.id})">Excluir</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    }
    else {
        alert("Nenhum documento encontrado para este cliente.");
    }
}

async function baixarArquivo(id) {
    try {
        const response = await fetch(`${URL_API}/download/${id}`);

        if (response.ok) {
            const blob = await response.blob();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;

            const contentDisposition = response.headers.get('Content-Disposition');
            let nomeArquivo = `arquivo_${id}`;

            if (contentDisposition) {
                const matchUtf8 = contentDisposition.match(/filename\*\s*=\s*(?:UTF-8|utf-8)''([^;]+)/i);

                if (matchUtf8) {
                    nomeArquivo = decodeURIComponent(matchUtf8[1].trim());
                }
                else if(matchNormal) {
                    const matchNormal = contentDisposition.match(/filename\s*=\s*"?([^";]+)"?/i);
                    nomeArquivo = matchNormal[1].trim();
                }
            }

            a.download = nomeArquivo;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        }
        else {
            alert("Erro ao baixar o arquivo.");
        }
    }
    catch (error) {
        console.error("Erro no download:", error);
        alert("Erro de conexão ao baixar o arquivo.");
    }
}

async function excluirArquivo(id) {
    if (!confirm("Tem certeza que deseja excluir este documento?"))
        return;

    const response = await fetch(`${URL_API}/excluir/${id}`, {
        method: "DELETE"
    });

    if (response.ok) {
        alert("Documento excluído com sucesso!");
        await buscarDocumentos();
    }
    else {
        alert("Erro ao excluir o documento.");
    }
}