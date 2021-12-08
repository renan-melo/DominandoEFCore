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
            ScriptGeralDoBancoDeDados();
            // MigraçõesJaAplicadas();
            // TodasMigracoes();
            // AplicarMigracaoEmTempodeExecucao();
            // MigracoesPendentes();
            // EnsureCreateAndDeleted();
            // GapDoEnsureCreated
            // HealthCheckBancoDeDados();
            //warmup
            // new DominandoEFCore.Data.ApplicationContext().Departamentos.AsNoTracking().Any();
            // GerenciarEstadoDaConexao(true);
            // GerenciarEstadoDaConexao(false);
        }

        #region ""
        #endregion


        #region "Gerando o script de criação SQL do modelo de dados"

        static void ScriptGeralDoBancoDeDados()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();
            Console.WriteLine(script);
        }
        #endregion

        #region "Recuperando migrações aplicadas em sua aplicação(Leitura)"
        //dotnet ef migrations add rg --context ApplicationContext (Comando para acrescentar mais uma migration)
        //dotnet ef migrations list --context ApplicationContext
        static void MigraçõesJaAplicadas()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            var migracoes = db.Database.GetAppliedMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach(var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }
        #endregion

        #region "Recuperando todas as migrações existentes em sua aplicação(Leitura)" 
        //dotnet ef migrations add rg --context ApplicationContext (Comando para acrescentar mais uma migration)
        static void TodasMigracoes()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            var migracoes = db.Database.GetMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach(var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }
        #endregion

        #region "Forçando uma migração"
        static void AplicarMigracaoEmTempodeExecucao()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            db.Database.Migrate();
        }
        #endregion

        #region "Detectando migrações pendentes"
        static void MigracoesPendentes()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();
            var migracoesPendentes = db.Database.GetPendingMigrations();
            Console.WriteLine($"Total: {migracoesPendentes.Count()}");
            foreach (var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }
        #endregion

        #region "Tipos de comandos em script SQL"
        static void ExecuteSQL()
        {
            using var db = new DominandoEFCore.Data.ApplicationContext();

            // Primeira Opção
            using (var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            //Segunda Opção
            var descricao = "TESTE";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            //Terceira Opção
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }
        #endregion

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
