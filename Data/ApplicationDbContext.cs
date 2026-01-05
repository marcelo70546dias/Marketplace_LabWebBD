using System;
using System.Collections.Generic;
using Marketplace_LabWebBD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Marketplace_LabWebBD.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Anuncio> Anuncios { get; set; }

    public virtual DbSet<Carro> Carros { get; set; }

    public virtual DbSet<Combustivel> Combustivels { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<Comprador> Compradors { get; set; }

    public virtual DbSet<Configuracao_Sistema> Configuracao_Sistemas { get; set; }

    public virtual DbSet<Cria> Cria { get; set; }

    public virtual DbSet<Filtro_Favorito> Filtro_Favoritos { get; set; }

    public virtual DbSet<Foto> Fotos { get; set; }

    public virtual DbSet<Historico_Reserva> Historico_Reservas { get; set; }

    public virtual DbSet<Log_Admin> Log_Admins { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<Modera> Moderas { get; set; }

    public virtual DbSet<Preferencia> Preferencias { get; set; }

    public virtual DbSet<Utilizador> Utilizadors { get; set; }

    public virtual DbSet<Vendedor> Vendedors { get; set; }

    public virtual DbSet<Visita> Visita { get; set; }

    public virtual DbSet<PedidoPromocaoAdmin> PedidoPromocaoAdmins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.ID_Utilizador).HasName("PK__Admin__020698172C794F51");

            entity.ToTable("Admin");

            entity.Property(e => e.ID_Utilizador).ValueGeneratedNever();

            entity.HasOne(d => d.ID_UtilizadorNavigation).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.ID_Utilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Admin__ID_Utiliz__5441852A");
        });

        modelBuilder.Entity<Anuncio>(entity =>
        {
            entity.HasKey(e => e.ID_Anuncio).HasName("PK__Anuncio__D8875FB6F59A892A");

            entity.ToTable("Anuncio");

            entity.HasIndex(e => e.Data_Publicacao, "IDX_Anuncio_Data").IsDescending();

            entity.HasIndex(e => e.Estado_Anuncio, "IDX_Anuncio_Estado");

            entity.HasIndex(e => e.Preco, "IDX_Anuncio_Preco");

            entity.Property(e => e.Data_Atualizacao).HasColumnType("datetime");
            entity.Property(e => e.Data_Publicacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.Estado_Anuncio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Ativo");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Prazo_Reserva_Dias).HasDefaultValue(7);
            entity.Property(e => e.Preco).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_CarroNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.ID_Carro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__ID_Carr__7C4F7684");

            entity.HasOne(d => d.ID_VendedorNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.ID_Vendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Anuncio__ID_Vend__7D439ABD");

            entity.HasOne(d => d.Reservado_PorNavigation).WithMany(p => p.Anuncios)
                .HasForeignKey(d => d.Reservado_Por)
                .HasConstraintName("FK__Anuncio__Reserva__7E37BEF6");
        });

        modelBuilder.Entity<Carro>(entity =>
        {
            entity.HasKey(e => e.ID_Carro).HasName("PK__Carro__8D68D2DEF25E62E4");

            entity.ToTable("Carro");

            entity.HasIndex(e => e.Ano, "IDX_Carro_Ano");

            entity.Property(e => e.Caixa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Cor)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_CombustivelNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.ID_Combustivel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__ID_Combus__70DDC3D8");

            entity.HasOne(d => d.ID_ModeloNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.ID_Modelo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__ID_Modelo__6FE99F9F");

            entity.HasOne(d => d.ID_VendedorNavigation).WithMany(p => p.Carros)
                .HasForeignKey(d => d.ID_Vendedor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Carro__ID_Vended__71D1E811");
        });

        modelBuilder.Entity<Combustivel>(entity =>
        {
            entity.HasKey(e => e.ID_Combustivel).HasName("PK__Combusti__F8AAF41E8A93ED39");

            entity.ToTable("Combustivel");

            entity.HasIndex(e => e.Tipo, "UQ__Combusti__8E762CB418F0EE7E").IsUnique();

            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.ID_Compra).HasName("PK__Compra__A9D5994E525465B7");

            entity.ToTable("Compra");

            entity.HasIndex(e => e.Data, "IDX_Compra_Data");

            entity.Property(e => e.Data).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Estado_Pagamento)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pendente");
            entity.Property(e => e.Metodo_Pagamento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notas).HasColumnType("text");
            entity.Property(e => e.Preco).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ID_AnuncioNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.ID_Anuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__ID_Anunc__0B91BA14");

            entity.HasOne(d => d.ID_CompradorNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.ID_Comprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compra__ID_Compr__0A9D95DB");
        });

        modelBuilder.Entity<Comprador>(entity =>
        {
            entity.HasKey(e => e.ID_Comprador).HasName("PK__Comprado__5E8ABD9D25E4FB23");

            entity.ToTable("Comprador");

            entity.Property(e => e.Notificacoes_Email).HasDefaultValue(true);
            entity.Property(e => e.Notificacoes_Push).HasDefaultValue(true);

            entity.HasOne(d => d.ID_UtilizadorNavigation).WithMany(p => p.Compradors)
                .HasForeignKey(d => d.ID_Utilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comprador__ID_Ut__59FA5E80");
        });

        modelBuilder.Entity<Configuracao_Sistema>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Configur__3214EC2779958503");

            entity.ToTable("Configuracao_Sistema");

            entity.Property(e => e.Descricao_Sobre).HasColumnType("text");
            entity.Property(e => e.Email_Contacto)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Horario_Suporte).HasColumnType("text");
            entity.Property(e => e.Nome_Marketplace)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Politica_Privacidade).HasColumnType("text");
            entity.Property(e => e.Telefone_Contacto)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Termos_Uso).HasColumnType("text");
        });

        modelBuilder.Entity<Cria>(entity =>
        {
            entity.HasKey(e => new { e.ID_Admin, e.ID_Utilizador }).HasName("PK__Cria__09D50EE74F4EDA6F");

            entity.Property(e => e.Data).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ID_AdminNavigation).WithMany(p => p.Cria)
                .HasForeignKey(d => d.ID_Admin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cria__ID_Admin__14270015");

            entity.HasOne(d => d.ID_UtilizadorNavigation).WithMany(p => p.Cria)
                .HasForeignKey(d => d.ID_Utilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cria__ID_Utiliza__151B244E");
        });

        modelBuilder.Entity<Filtro_Favorito>(entity =>
        {
            entity.HasKey(e => e.ID_Filtro).HasName("PK__Filtro_F__931D1A29C629E33F");

            entity.ToTable("Filtro_Favorito");

            entity.Property(e => e.Caixa)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Data_Criacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Nome_Filtro)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Preco_Max).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Preco_Min).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.ID_CombustivelNavigation).WithMany(p => p.Filtro_Favoritos)
                .HasForeignKey(d => d.ID_Combustivel)
                .HasConstraintName("FK__Filtro_Fa__ID_Co__1BC821DD");

            entity.HasOne(d => d.ID_CompradorNavigation).WithMany(p => p.Filtro_Favoritos)
                .HasForeignKey(d => d.ID_Comprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Filtro_Fa__ID_Co__18EBB532");

            entity.HasOne(d => d.ID_MarcaNavigation).WithMany(p => p.Filtro_Favoritos)
                .HasForeignKey(d => d.ID_Marca)
                .HasConstraintName("FK__Filtro_Fa__ID_Ma__19DFD96B");

            entity.HasOne(d => d.ID_ModeloNavigation).WithMany(p => p.Filtro_Favoritos)
                .HasForeignKey(d => d.ID_Modelo)
                .HasConstraintName("FK__Filtro_Fa__ID_Mo__1AD3FDA4");
        });

        modelBuilder.Entity<Foto>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Foto__3214EC277AC42C3E");

            entity.ToTable("Foto");

            entity.Property(e => e.Fotografia).IsUnicode(false);
            entity.Property(e => e.Ordem).HasDefaultValue(1);

            entity.HasOne(d => d.ID_CarroNavigation).WithMany(p => p.Fotos)
                .HasForeignKey(d => d.ID_Carro)
                .HasConstraintName("FK__Foto__ID_Carro__75A278F5");
        });

        modelBuilder.Entity<Historico_Reserva>(entity =>
        {
            entity.HasKey(e => e.ID_Historico).HasName("PK__Historic__ECA88795102F448E");

            entity.ToTable("Historico_Reserva");

            entity.Property(e => e.Data_Inicio).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_AnuncioNavigation).WithMany(p => p.Historico_Reservas)
                .HasForeignKey(d => d.ID_Anuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__ID_An__25518C17");

            entity.HasOne(d => d.ID_CompradorNavigation).WithMany(p => p.Historico_Reservas)
                .HasForeignKey(d => d.ID_Comprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historico__ID_Co__2645B050");
        });

        modelBuilder.Entity<Log_Admin>(entity =>
        {
            entity.HasKey(e => e.ID_Log).HasName("PK__Log_Admi__2DBF3395F154EE25");

            entity.ToTable("Log_Admin");

            entity.Property(e => e.Data_Hora)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Descricao).HasColumnType("text");
            entity.Property(e => e.IP_Address)
                .HasMaxLength(45)
                .IsUnicode(false);
            entity.Property(e => e.Tipo_Acao)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_AdminNavigation).WithMany(p => p.Log_Admins)
                .HasForeignKey(d => d.ID_Admin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Log_Admin__ID_Ad__1F98B2C1");

            entity.HasOne(d => d.ID_Anuncio_AfetadoNavigation).WithMany(p => p.Log_Admins)
                .HasForeignKey(d => d.ID_Anuncio_Afetado)
                .HasConstraintName("FK__Log_Admin__ID_An__2180FB33");

            entity.HasOne(d => d.ID_Utilizador_AfetadoNavigation).WithMany(p => p.Log_Admins)
                .HasForeignKey(d => d.ID_Utilizador_Afetado)
                .HasConstraintName("FK__Log_Admin__ID_Ut__208CD6FA");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.ID_Marca).HasName("PK__Marca__9B8F8DB2BDA72356");

            entity.ToTable("Marca");

            entity.HasIndex(e => e.Nome, "UQ__Marca__7D8FE3B2485BF692").IsUnique();

            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.ID_Modelo).HasName("PK__Modelo__813C23729CB904A5");

            entity.ToTable("Modelo");

            entity.HasIndex(e => e.ID_Marca, "IDX_Carro_Marca");

            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_MarcaNavigation).WithMany(p => p.Modelos)
                .HasForeignKey(d => d.ID_Marca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modelo__ID_Marca__693CA210");
        });

        modelBuilder.Entity<Modera>(entity =>
        {
            entity.HasKey(e => new { e.ID_Admin, e.ID_Anuncio, e.Data }).HasName("PK__Modera__1F0A2AE08E6661C3");

            entity.ToTable("Modera");

            entity.Property(e => e.Data)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Acao)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Motivo).HasColumnType("text");

            entity.HasOne(d => d.ID_AdminNavigation).WithMany(p => p.Moderas)
                .HasForeignKey(d => d.ID_Admin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modera__ID_Admin__0F624AF8");

            entity.HasOne(d => d.ID_AnuncioNavigation).WithMany(p => p.Moderas)
                .HasForeignKey(d => d.ID_Anuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Modera__ID_Anunc__10566F31");
        });

        modelBuilder.Entity<Preferencia>(entity =>
        {
            entity.HasKey(e => new { e.ID_Utilizador, e.ID_Marca }).HasName("PK__Preferen__3BBE60CC9723BEEE");

            entity.Property(e => e.Data_Adicao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ID_MarcaNavigation).WithMany(p => p.Preferencia)
                .HasForeignKey(d => d.ID_Marca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Preferenc__ID_Ma__66603565");

            entity.HasOne(d => d.ID_UtilizadorNavigation).WithMany(p => p.Preferencia)
                .HasForeignKey(d => d.ID_Utilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Preferenc__ID_Ut__656C112C");
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__Utilizad__3214EC27EA0E8A85");

            entity.ToTable("Utilizador");

            entity.HasIndex(e => e.Email, "IDX_Usuario_Email");

            entity.HasIndex(e => e.Status, "IDX_Usuario_Status");

            entity.HasIndex(e => e.Username, "UQ__Utilizad__536C85E4E9090F69").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Utilizad__A9D105341B8CEC97").IsUnique();

            entity.Property(e => e.Bloqueado).HasDefaultValue(false);
            entity.Property(e => e.Contacto)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Data_Registo).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Data_Token).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Email_Validado).HasDefaultValue(false);
            entity.Property(e => e.Morada)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Motivo_Bloqueio).HasColumnType("text");
            entity.Property(e => e.Nome)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Ativo");
            entity.Property(e => e.Token_Validacao)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_Admin_BloqueioNavigation).WithMany(p => p.Utilizadors)
                .HasForeignKey(d => d.ID_Admin_Bloqueio)
                .HasConstraintName("FK__Utilizado__ID_Ad__5535A963");
        });

        modelBuilder.Entity<Vendedor>(entity =>
        {
            entity.HasKey(e => e.ID_Utilizador).HasName("PK__Vendedor__020698174C3EBC1E");

            entity.ToTable("Vendedor");

            entity.Property(e => e.ID_Utilizador).ValueGeneratedNever();
            entity.Property(e => e.Dados_Faturacao)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NIF)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_AprovacaoNavigation).WithMany(p => p.Vendedors)
                .HasForeignKey(d => d.ID_Aprovacao)
                .HasConstraintName("FK__Vendedor__ID_Apr__5EBF139D");

            entity.HasOne(d => d.ID_UtilizadorNavigation).WithOne(p => p.Vendedor)
                .HasForeignKey<Vendedor>(d => d.ID_Utilizador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vendedor__ID_Uti__5DCAEF64");
        });

        modelBuilder.Entity<Visita>(entity =>
        {
            entity.HasKey(e => e.ID_Visita).HasName("PK__Visita__015899C82633D33D");

            entity.Property(e => e.Data_Criacao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Data_Hora).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Pendente");
            entity.Property(e => e.Localizacao)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Observacoes).HasColumnType("text");

            entity.HasOne(d => d.ID_AnuncioNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.ID_Anuncio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__ID_Anunc__04E4BC85");

            entity.HasOne(d => d.ID_CompradorNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.ID_Comprador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Visita__ID_Compr__03F0984C");
        });

        modelBuilder.Entity<PedidoPromocaoAdmin>(entity =>
        {
            entity.HasKey(e => e.ID_Pedido).HasName("PK__Pedido_P__C5A30BC2");

            entity.ToTable("PedidoPromocaoAdmin");

            entity.Property(e => e.Data_Pedido)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.Data_Resposta)
                .HasColumnType("datetime");

            entity.Property(e => e.Tipo_Utilizador_Atual)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pendente");

            entity.Property(e => e.Justificacao)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.Property(e => e.Observacoes_Admin)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.ID_UtilizadorNavigation).WithMany()
                .HasForeignKey(d => d.ID_Utilizador)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ID_Admin_RespostaNavigation).WithMany()
                .HasForeignKey(d => d.ID_Admin_Resposta)
                .OnDelete(DeleteBehavior.SetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
