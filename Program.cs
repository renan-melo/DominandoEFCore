using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            // EnsureCreateAndDeleted();
            // GapDoEnsureCreated
            // HealthCheckBancoDeDados();
            //warmup
            new DominandoEFCore.Data.ApplicationContext().Departamentos.AsNoTracking().Any();
            GerenciarEstadoDaConexao(true);
            GerenciarEstadoDaConexao(false);
        }

        static void ExecuteSQL()
        { 
            using var db = new DominandoEFCore.Data.ApplicationContext();

            // Primeira Opção
            using(var cmd = db.Database.GetDbConnection().CreateCommand()) {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoDaConexao)
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();
            conexao.StateChange += (_, __) => ++_count;

            if (gerenciarEstadoDaConexao)
            {
                conexao.Open();
            }

            for (int i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoDaConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }
        static void HealthCheckBancoDeDados()
        {
            //Este metodo (CanConnect) visa verificar se esta sendo feito uma conecção com o banco
            using var db = new DominandoEFCore.Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect)
            {
                Console.WriteLine("Posso me conectar");
            }
            else
            {
                Console.WriteLine("Não posso me conectar");
            }
        }
        static void EnsureCreateAndDeleted()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            db.Database.EnsureCreated();
            // db.Database.EnsureDeleted();

        }
        static void GapDoEnsureCreated()
        {
            using var db1 = new DominandoEFCore.Data.ApplicationContext();
            using var db2 = new DominandoEFCore.Data.ApplicationContextCidade();
            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }
    }
}
