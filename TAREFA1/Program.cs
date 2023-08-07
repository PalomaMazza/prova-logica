using System;
using System.IO;

class Program
{
    static void Main()
    {
        string arquivoOriginal = "mapa.csv";
        string arquivoAlterado = "mapa_formatado.csv";

        try
        {
            //Apesar de hoje o arquivo estar com o separador ";", estou validando mais de um tipo de separador por precaução.
            char[] separadores = new char[] { ';', '|' };

            //Lê todas as linhas do arquivo original
            string[] linhas = File.ReadAllLines(arquivoOriginal);

            //Cria um StreamWriter para gravar o arquivo alterado
            using (StreamWriter writer = new StreamWriter(arquivoAlterado))
            {
                foreach (string linha in linhas)
                {
                    //Separa as colunas pelo caractere de separação
                    string[] colunas = linha.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                    //Verifica se a linha possui a quantidade esperada de colunas
                    if (colunas.Length >= 2)
                    {
                        //Obtém o valor da população no último censo
                        string populacaoString = colunas[1];

                        //Formata o valor para ter 0 casas decimais e separador de milhar
                        if (long.TryParse(populacaoString, out long populacao))
                        {
                            string populacaoFormatada = populacao.ToString("N0");
                            colunas[1] = populacaoFormatada;
                        }
                    }

                    //Junta as colunas novamente em uma linha e grava no arquivo alterado
                    writer.WriteLine(string.Join(";", colunas));
                }
            }

            Console.WriteLine("Arquivo formatado e gravado com sucesso!");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Arquivo 'mapa.csv' não encontrado!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
        }
    }
}
