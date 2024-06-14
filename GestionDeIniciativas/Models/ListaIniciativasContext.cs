using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GestionDeIniciativas.Models;

public partial class ListaIniciativasContext : DbContext
{
    public ListaIniciativasContext()
    {
    }

    public ListaIniciativasContext(DbContextOptions<ListaIniciativasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BloqueTiempo> BloqueTiempos { get; set; }

    public virtual DbSet<Iniciativa> Iniciativas { get; set; }

    public virtual DbSet<Tarea> Tareas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        { 
        //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        //=> optionsBuilder.UseSqlServer("server=GONZALO\\SQLEXPRESS; database=ListaIniciativas; integrated security=true; Encrypt=false; Trusted_Connection=true; ");

        }
    }


protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BloqueTiempo>(entity =>
        {
            entity.HasKey(e => e.TiempoId).HasName("PK__BloqueTi__1491EB9DFD9B2020");

            entity.ToTable("BloqueTiempo");

            entity.Property(e => e.TiempoId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Tiempo_ID");
            entity.Property(e => e.DiaMes)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Dia_mes");
            entity.Property(e => e.DiaSemana)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Dia_semana");
            entity.Property(e => e.Progreso)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TareaId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Tarea_ID");

            entity.HasOne(d => d.Tarea).WithMany(p => p.BloqueTiempos)
                .HasForeignKey(d => d.TareaId)
                .HasConstraintName("fk_tarea");
        });

        modelBuilder.Entity<Iniciativa>(entity =>
        {
            entity.HasKey(e => e.IniciativaId).HasName("PK__Iniciati__01A04AEBC244807A");

            entity.ToTable("Iniciativa");

            entity.Property(e => e.IniciativaId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Iniciativa_ID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Prioridad)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Tipo)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Usuario_ID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Iniciativas)
                .HasForeignKey(d => d.UsuarioId)
                .HasConstraintName("fk_usuario");
        });

        modelBuilder.Entity<Tarea>(entity =>
        {
            entity.HasKey(e => e.TareaId).HasName("PK__Tarea__327AB98AF754AB4B");

            entity.ToTable("Tarea");

            entity.Property(e => e.TareaId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Tarea_ID");
            entity.Property(e => e.Categoría)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IniciativaId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Iniciativa_ID");
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Iniciativa).WithMany(p => p.Tareas)
                .HasForeignKey(d => d.IniciativaId)
                .HasConstraintName("fk_iniciativa");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuario__7711133570AB1D23");

            entity.ToTable("Usuario");

            entity.Property(e => e.UsuarioId)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Usuario_ID");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(255)
                .IsUnicode(false);
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}






