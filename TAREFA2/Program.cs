using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string arquivoOriginal = "mapa.csv";
        string arquivoOrdenado = "mapa_ordenado.csv";

        try
        {
            char[] separadores = new char[] { ';', '|' };

            //Lê todas as linhas do arquivo original
            string[] linhas = File.ReadAllLines(arquivoOriginal);

            //Cria uma estrutura de "lista de listas" para armazenar os dados como uma tabela
            List<List<string>> tabela = new List<List<string>>();

            //Separa as colunas de cada linha e adiciona à tabela
            foreach (string linha in linhas)
            {
                string[] colunas = linha.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                tabela.Add(new List<string>(colunas));
            }

            //Ordena a tabela em ordem decrescente pela coluna "População no último censo"
            BubbleSort(tabela);

            //Cria um StreamWriter para gravar o arquivo ordenado
            using (StreamWriter writer = new StreamWriter(arquivoOrdenado))
            {
                foreach (List<string> linha in tabela)
                {
                    //Junta as colunas de volta em uma linha e grava no arquivo ordenado
                    writer.WriteLine(string.Join(";", linha));
                }
            }

            Console.WriteLine("Arquivo ordenado e gravado com sucesso!");
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

    static void BubbleSort(List<List<string>> tabela)
    {
        int n = tabela.Count;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (long.TryParse(tabela[j][1], out long populacao1) && long.TryParse(tabela[j + 1][1], out long populacao2))
                {
                    if (populacao1 < populacao2)
                    {
                        //Troca as linhas de posição para ordenar em ordem decrescente
                        List<string> temp = tabela[j];
                        tabela[j] = tabela[j + 1];
                        tabela[j + 1] = temp;
                    }
                }
            }
        }
    }
}
