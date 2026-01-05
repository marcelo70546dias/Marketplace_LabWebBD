using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.TempModels;

public partial class TempContext : DbContext
{
    public TempContext()
    {
    }

    public TempContext(DbContextOptions<TempContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Anuncio> Anuncios { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Carro> Carros { get; set; }

    public virtual DbSet<Combustivel> Combustivels { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Comprador> Compradors { get; set; }

    public virtual DbSet<ConfiguracaoSistema> ConfiguracaoSistemas { get; set; }

    public virtual DbSet<Crium> Cria { get; set; }

    public virtual DbSet<FiltroFavorito> FiltroFavoritos { get; set; }

    public virtual DbSet<Foto> Fotos { get; set; }

    public virtual DbSet<HistoricoReserva> HistoricoReservas { get; set; }

    public virtual DbSet<LogAdmin> LogAdmins { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<Modera> Moderas { get; set; }

    public virtual DbSet<Preferencia> Preferencias { get; set; }

    public virtual DbSet<Vendedor> Vendedors { get; set; }

    public virtual DbSet<Visitum> Visita { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.IdUtilizador).HasName("PK__Admin__020698172C794F51");

            entity.ToTable("Admin");

            entity.Property(e => e.IdUtilizador)
                .ValueGeneratedNever()
                .HasColumnName("ID_Utilizador");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_AspNetUsers");
        });

        modelBuilder.Entity<Anuncio>(entity =>
        {
            entity.HasKey(e => e.IdAnuncio).HasName("PK__Anuncio__D8875FB6F59A892A");

            entity.ToTable("Anuncio");

            entity.HasIndex(e => e.DataPublicacao, "IDX_Anuncio_Data").IsDescending();

            entity.HasIndex(e => e.EstadoAnuncio, "IDX_Anuncio_Estado");

            entity.HasIndex(e => e.Preco, "IDX_Anuncio_Preco");

            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.DataAtualizacao)
                .HasColumnType("datetime")
                .HasColumnName("Data_Atualizacao");
            entity.Property(e => e.DataPublicacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Publicacao");
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.EstadoAnuncio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Ativo")
                .HasColumnName("Estado_Anuncio");
            entity.Property(e => e.IdCarro).HasColumnName("ID_Carro");
            entity.Property(e => e.IdVendedor).HasColumnName("ID_Vendedor");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.PrazoReservaDias)
                .HasDefaultValue(7)
                .HasColumnName("Prazo_Reserva_Dias");
            entity.Property(e => e.Preco).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReservadoAte).HasColumnName("Reservado_Ate");
            entity.Property(e => e.ReservadoPor).HasColumnName("Reservado_Por");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdCarroNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.IdCarro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__ID_Carr__7C4F7684");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__ID_Vend__7D439ABD");

            entity.HasOne(d => d.ReservadoPorNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.ReservadoPor)
                .HasConstraintName("FK__Anuncio__Reserva__7E37BEF6");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.Property(e => e.DataAprovacaoVendedor).HasColumnName("Data_Aprovacao_Vendedor");
            entity.Property(e => e.DataBloqueio).HasColumnName("Data_Bloqueio");
            entity.Property(e => e.DataRegisto).HasColumnName("Data_Registo");
            entity.Property(e => e.DataToken).HasColumnName("Data_Token");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.EmailValidado).HasColumnName("Email_Validado");
            entity.Property(e => e.IdAdminBloqueio).HasColumnName("ID_Admin_Bloqueio");
            entity.Property(e => e.IdAprovacaoVendedor).HasColumnName("ID_Aprovacao_Vendedor");
            entity.Property(e => e.MotivoBloqueio).HasColumnName("Motivo_Bloqueio");
            entity.Property(e => e.MotivoRejeicaoVendedor).HasColumnName("Motivo_Rejeicao_Vendedor");
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.TokenValidacao).HasColumnName("Token_Validacao");
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Carro>(entity =>
        {
            entity.HasKey(e => e.IdCarro).HasName("PK__Carro__8D68D2DEF25E62E4");

            entity.ToTable("Carro");

            entity.HasIndex(e => e.Ano, "IDX_Carro_Ano");

            entity.Property(e => e.IdCarro).HasColumnName("ID_Carro");
            entity.Property(e => e.Caixa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Cor)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IdCombustivel).HasColumnName("ID_Combustivel");
            entity.Property(e => e.IdModelo).HasColumnName("ID_Modelo");
            entity.Property(e => e.IdVendedor).HasColumnName("ID_Vendedor");

            entity.HasOne(d => d.IdCombustivelNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.IdCombustivel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__ID_Combus__70DDC3D8");

            entity.HasOne(d => d.IdModeloNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.IdModelo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__ID_Modelo__6FE99F9F");

            entity.HasOne(d => d.IdVendedorNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.IdVendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__ID_Vended__71D1E811");
        });

        modelBuilder.Entity<Combustivel>(entity =>
        {
            entity.HasKey(e => e.IdCombustivel).HasName("PK__Combusti__F8AAF41E8A93ED39");

            entity.ToTable("Combustivel");

            entity.HasIndex(e => e.Tipo, "UQ__Combusti__8E762CB418F0EE7E").IsUnique();

            entity.Property(e => e.IdCombustivel).HasColumnName("ID_Combustivel");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compra__A9D5994E525465B7");

            entity.ToTable("Compra");

            entity.HasIndex(e => e.Data, "IDX_Compra_Data");

            entity.Property(e => e.IdCompra).HasColumnName("ID_Compra");
            entity.Property(e => e.Data).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EstadoPagamento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendente")
                .HasColumnName("Estado_Pagamento");
            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.IdComprador).HasColumnName("ID_Comprador");
            entity.Property(e => e.MetodoPagamento)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Metodo_Pagamento");
            entity.Property(e => e.Notas).HasColumnType("text");
            entity.Property(e => e.Preco).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__ID_Anunc__0B91BA14");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__ID_Compr__0A9D95DB");
        });

        modelBuilder.Entity<Comprador>(entity =>
        {
            entity.HasKey(e => e.IdComprador).HasName("PK__Comprado__5E8ABD9D25E4FB23");

            entity.ToTable("Comprador");

            entity.Property(e => e.IdComprador).HasColumnName("ID_Comprador");
            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");
            entity.Property(e => e.NotificacoesEmail)
                .HasDefaultValue(true)
                .HasColumnName("Notificacoes_Email");
            entity.Property(e => e.NotificacoesPush)
                .HasDefaultValue(true)
                .HasColumnName("Notificacoes_Push");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithMany(p => p.Compradors)
                .HasForeignKey(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comprador_AspNetUsers");
        });

        modelBuilder.Entity<ConfiguracaoSistema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Configur__3214EC2779958503");

            entity.ToTable("Configuracao_Sistema");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DescricaoSobre)
                .HasColumnType("text")
                .HasColumnName("Descricao_Sobre");
            entity.Property(e => e.EmailContacto)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("Email_Contacto");
            entity.Property(e => e.HorarioSuporte)
                .HasColumnType("text")
                .HasColumnName("Horario_Suporte");
            entity.Property(e => e.NomeMarketplace)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("Nome_Marketplace");
            entity.Property(e => e.PoliticaPrivacidade)
                .HasColumnType("text")
                .HasColumnName("Politica_Privacidade");
            entity.Property(e => e.TelefoneContacto)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Telefone_Contacto");
            entity.Property(e => e.TermosUso)
                .HasColumnType("text")
                .HasColumnName("Termos_Uso");
        });

        modelBuilder.Entity<Crium>(entity =>
        {
            entity.HasKey(e => new { e.IdAdmin, e.IdUtilizador }).HasName("PK__Cria__09D50EE74F4EDA6F");

            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin");
            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");
            entity.Property(e => e.Data).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.IdAdminNavigation).WithMany(p => p.Cria)
                .HasForeignKey(d => d.IdAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cria__ID_Admin__14270015");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithMany(p => p.Cria)
                .HasForeignKey(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cria_AspNetUsers");
        });

        modelBuilder.Entity<FiltroFavorito>(entity =>
        {
            entity.HasKey(e => e.IdFiltro).HasName("PK__Filtro_F__931D1A29C629E33F");

            entity.ToTable("Filtro_Favorito");

            entity.Property(e => e.IdFiltro).HasColumnName("ID_Filtro");
            entity.Property(e => e.AnoMax).HasColumnName("Ano_Max");
            entity.Property(e => e.AnoMin).HasColumnName("Ano_Min");
            entity.Property(e => e.Caixa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");
            entity.Property(e => e.IdCombustivel).HasColumnName("ID_Combustivel");
            entity.Property(e => e.IdComprador).HasColumnName("ID_Comprador");
            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");
            entity.Property(e => e.IdModelo).HasColumnName("ID_Modelo");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.NomeFiltro)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Nome_Filtro");
            entity.Property(e => e.PrecoMax)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Preco_Max");
            entity.Property(e => e.PrecoMin)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Preco_Min");
            entity.Property(e => e.QuilometragemMax).HasColumnName("Quilometragem_Max");

            entity.HasOne(d => d.IdCombustivelNavigation).WithMany(p => p.FiltroFavoritos)
                .HasForeignKey(d => d.IdCombustivel)
                .HasConstraintName("FK__Filtro_Fa__ID_Co__1BC821DD");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.FiltroFavoritos)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Filtro_Fa__ID_Co__18EBB532");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.FiltroFavoritos)
                .HasForeignKey(d => d.IdMarca)
                .HasConstraintName("FK__Filtro_Fa__ID_Ma__19DFD96B");

            entity.HasOne(d => d.IdModeloNavigation).WithMany(p => p.FiltroFavoritos)
                .HasForeignKey(d => d.IdModelo)
                .HasConstraintName("FK__Filtro_Fa__ID_Mo__1AD3FDA4");
        });

        modelBuilder.Entity<Foto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Foto__3214EC277AC42C3E");

            entity.ToTable("Foto");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Fotografia).IsUnicode(false);
            entity.Property(e => e.IdCarro).HasColumnName("ID_Carro");
            entity.Property(e => e.Ordem).HasDefaultValue(1);

            entity.HasOne(d => d.IdCarroNavigation).WithMany(p => p.Fotos)
                .HasForeignKey(d => d.IdCarro)
                .HasConstraintName("FK__Foto__ID_Carro__75A278F5");
        });

        modelBuilder.Entity<HistoricoReserva>(entity =>
        {
            entity.HasKey(e => e.IdHistorico).HasName("PK__Historic__ECA88795102F448E");

            entity.ToTable("Historico_Reserva");

            entity.Property(e => e.IdHistorico).HasColumnName("ID_Historico");
            entity.Property(e => e.DataFim).HasColumnName("Data_Fim");
            entity.Property(e => e.DataInicio)
                .HasColumnType("datetime")
                .HasColumnName("Data_Inicio");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.IdComprador).HasColumnName("ID_Comprador");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.HistoricoReservas)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__ID_An__25518C17");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.HistoricoReservas)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__ID_Co__2645B050");
        });

        modelBuilder.Entity<LogAdmin>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("PK__Log_Admi__2DBF3395F154EE25");

            entity.ToTable("Log_Admin");

            entity.Property(e => e.IdLog).HasColumnName("ID_Log");
            entity.Property(e => e.DataHora)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Hora");
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin");
            entity.Property(e => e.IdAnuncioAfetado).HasColumnName("ID_Anuncio_Afetado");
            entity.Property(e => e.IdUtilizadorAfetado).HasColumnName("ID_Utilizador_Afetado");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .IsUnicode(false)
                .HasColumnName("IP_Address");
            entity.Property(e => e.TipoAcao)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("Tipo_Acao");

            entity.HasOne(d => d.IdAdminNavigation).WithMany(p => p.LogAdmins)
                .HasForeignKey(d => d.IdAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Log_Admin__ID_Ad__1F98B2C1");

            entity.HasOne(d => d.IdAnuncioAfetadoNavigation).WithMany(p => p.LogAdmins)
                .HasForeignKey(d => d.IdAnuncioAfetado)
                .HasConstraintName("FK__Log_Admin__ID_An__2180FB33");

            entity.HasOne(d => d.IdUtilizadorAfetadoNavigation).WithMany(p => p.LogAdmins)
                .HasForeignKey(d => d.IdUtilizadorAfetado)
                .HasConstraintName("FK_LogAdmin_AspNetUsers");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.IdMarca).HasName("PK__Marca__9B8F8DB2BDA72356");

            entity.ToTable("Marca");

            entity.HasIndex(e => e.Nome, "UQ__Marca__7D8FE3B2485BF692").IsUnique();

            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.IdModelo).HasName("PK__Modelo__813C23729CB904A5");

            entity.ToTable("Modelo");

            entity.HasIndex(e => e.IdMarca, "IDX_Carro_Marca");

            entity.Property(e => e.IdModelo).HasColumnName("ID_Modelo");
            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Modelos)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modelo__ID_Marca__693CA210");
        });

        modelBuilder.Entity<Modera>(entity =>
        {
            entity.HasKey(e => new { e.IdAdmin, e.IdAnuncio, e.Data }).HasName("PK__Modera__1F0A2AE08E6661C3");

            entity.ToTable("Modera");

            entity.Property(e => e.IdAdmin).HasColumnName("ID_Admin");
            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.Data)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Acao)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Motivo).HasColumnType("text");

            entity.HasOne(d => d.IdAdminNavigation).WithMany(p => p.Moderas)
                .HasForeignKey(d => d.IdAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modera__ID_Admin__0F624AF8");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Moderas)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modera__ID_Anunc__10566F31");
        });

        modelBuilder.Entity<Preferencia>(entity =>
        {
            entity.HasKey(e => new { e.IdUtilizador, e.IdMarca }).HasName("PK__Preferen__3BBE60CC9723BEEE");

            entity.Property(e => e.IdUtilizador).HasColumnName("ID_Utilizador");
            entity.Property(e => e.IdMarca).HasColumnName("ID_Marca");
            entity.Property(e => e.DataAdicao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("Data_Adicao");

            entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Preferencia)
                .HasForeignKey(d => d.IdMarca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Preferenc__ID_Ma__66603565");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithMany(p => p.Preferencia)
                .HasForeignKey(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Preferencias_AspNetUsers");
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.HasKey(e => e.IdUtilizador).HasName("PK__Vendedor__020698174C3EBC1E");

            entity.ToTable("Vendedor");

            entity.Property(e => e.IdUtilizador)
                .ValueGeneratedNever()
                .HasColumnName("ID_Utilizador");
            entity.Property(e => e.DadosFaturacao)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Dados_Faturacao");
            entity.Property(e => e.DataAprovacao).HasColumnName("Data_Aprovacao");
            entity.Property(e => e.IdAprovacao).HasColumnName("ID_Aprovacao");
            entity.Property(e => e.Nif)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NIF");
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdAprovacaoNavigation).WithMany(p => p.Vendedors)
                .HasForeignKey(d => d.IdAprovacao)
                .HasConstraintName("FK__Vendedor__ID_Apr__5EBF139D");

            entity.HasOne(d => d.IdUtilizadorNavigation).WithOne(p => p.Vendedor)
                .HasForeignKey<Vendedor>(d => d.IdUtilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vendedor_AspNetUsers");
        });

        modelBuilder.Entity<Visitum>(entity =>
        {
            entity.HasKey(e => e.IdVisita).HasName("PK__Visita__015899C82633D33D");

            entity.Property(e => e.IdVisita).HasColumnName("ID_Visita");
            entity.Property(e => e.DataCriacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Data_Criacao");
            entity.Property(e => e.DataHora)
                .HasColumnType("datetime")
                .HasColumnName("Data_Hora");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendente");
            entity.Property(e => e.IdAnuncio).HasColumnName("ID_Anuncio");
            entity.Property(e => e.IdComprador).HasColumnName("ID_Comprador");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Observacoes).HasColumnType("text");

            entity.HasOne(d => d.IdAnuncioNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdAnuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__ID_Anunc__04E4BC85");

            entity.HasOne(d => d.IdCompradorNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdComprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__ID_Compr__03F0984C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
