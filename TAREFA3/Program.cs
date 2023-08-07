using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

class Program
{
    static async Task Main()
    {
        char[] separadores = new char[] { ',', ';', '|' };

        string arquivoOriginal = "CEPs.csv";
        string arquivoPreenchido = "CEPs_preenchidos.csv";

        try
        {
            string[] linhas = await File.ReadAllLinesAsync(arquivoOriginal);

            //Obtém o cabeçalho para saber as colunas
            string cabecalho = linhas[0];
            string[] colunas = cabecalho.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            //Encontra a posição correta da coluna "CEP"
            int posicaoCEP = ObterPosicaoCEP(colunas);

            List<string> linhasPreenchidas = new List<string>();

            linhasPreenchidas.Add(cabecalho);

            //Faz a requisição à API ViaCEP para obter os dados de cidade e bairro para cada CEP
            using (HttpClient httpClient = new HttpClient())
            {
                for (int i = 1; i < linhas.Length; i++)
                {
                    string linha = linhas[i];
                    string[] valores = linha.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                    string cep = valores[posicaoCEP]; //Obtém o CEP da coluna correta

                    if (IsValidCEP(cep)) //Verifica se o CEP é válido
                    {
                        string endereco = await GetEnderecoFromViaCEP(httpClient, cep);

                        string linhaPreenchida = string.Join(";", valores) + ";" + endereco;

                        linhasPreenchidas.Add(linhaPreenchida);
                    }
                    else
                    {
                        linhasPreenchidas.Add(linha);
                    }
                }
            }

            //Grava a lista de linhas preenchidas em um novo arquivo
            await File.WriteAllLinesAsync(arquivoPreenchido, linhasPreenchidas);

            Console.WriteLine("Arquivo preenchido e gravado com sucesso!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Arquivo 'CEPs.csv' não encontrado!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }
    }

    static int ObterPosicaoCEP(string[] colunas)
    {
        for (int i = 0; i < colunas.Length; i++)
        {
            if (colunas[i].Trim().Equals("CEP", StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        throw new Exception("Cabeçalho 'CEP' não encontrado!");
    }

    static bool IsValidCEP(string cep)
    {
        string cepLimpo = new string(cep.Where(char.IsDigit).ToArray());
        return cepLimpo.Length == 8;
    }

    static async Task<string> GetEnderecoFromViaCEP(HttpClient httpClient, string cep)
    {
        string url = $"https://viacep.com.br/ws/{cep}/json/";

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var enderecoData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

                // Obtém os valores específicos retornados pela API
                string logradouro = enderecoData.ContainsKey("logradouro") ? enderecoData["logradouro"] : "";
                string complemento = enderecoData.ContainsKey("complemento") ? enderecoData["complemento"] : "";
                string bairro = enderecoData.ContainsKey("bairro") ? enderecoData["bairro"] : "";
                string localidade = enderecoData.ContainsKey("localidade") ? enderecoData["localidade"] : "";
                string uf = enderecoData.ContainsKey("uf") ? enderecoData["uf"] : "";
                string unidade = enderecoData.ContainsKey("unidade") ? enderecoData["unidade"] : "";
                string ibge = enderecoData.ContainsKey("ibge") ? enderecoData["ibge"] : "";
                string gia = enderecoData.ContainsKey("gia") ? enderecoData["gia"] : "";

                //Concatena todos os valores em uma única string
                string endereco = string.Join(";", logradouro, complemento, bairro, localidade, uf, unidade, ibge, gia);

                return endereco;
            }
        }
        catch (HttpRequestException)
        {
            return "";
        }

        return "";
    }
}
