using Microsoft.EntityFrameworkCore;
using SalovetAPI.Models;
namespace SalovetAPI.Data
{
    public class SalovetDbContext : DbContext
    {
        public SalovetDbContext(DbContextOptions<SalovetDbContext> options)
     : base(options)
        {
        }

        // DbSets para cada tabla
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Profesional> Profesionales { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }
        public DbSet<Mascota> Mascotas { get; set; }
        public DbSet<Cita> Citas { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Registro> Registros { get; set; }
        public DbSet<RegistroMascota> RegistroMascotas { get; set; }
        public DbSet<Servicio> Servicios { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==================== CLIENTES ====================
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.ToTable("clientes");
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
                entity.Property(e => e.NombreCli).HasColumnName("nombre_cli").HasMaxLength(200).IsRequired();
                entity.Property(e => e.ApeCli).HasColumnName("ape_cli").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Edad).HasColumnName("edad").IsRequired();
                entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Tel).HasColumnName("tel").HasMaxLength(100);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Username).HasColumnName("username").HasMaxLength(250).IsRequired();
                entity.Property(e => e.Pass).HasColumnName("pass").HasMaxLength(200);
                entity.Property(e => e.Primero).HasColumnName("primero");
                entity.Property(e => e.Profesional).HasColumnName("profesional").IsRequired();
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente");

                entity.Property(e => e.IdProf).HasColumnName("id_prof");

                entity.HasOne(e => e.Cliente)
                      .WithMany()
                      .HasForeignKey(e => e.IdCliente)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ProfesionalNav)
                      .WithMany()
                      .HasForeignKey(e => e.IdProf)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== PROFESIONALES ====================
            modelBuilder.Entity<Profesional>(entity =>
            {
                entity.ToTable("profesionales");
                entity.HasKey(e => e.IdProf);
                entity.Property(e => e.IdProf).HasColumnName("id_prof");
                entity.Property(e => e.NomProf).HasColumnName("nom_prof").HasMaxLength(100).IsRequired();
                entity.Property(e => e.ApeProf).HasColumnName("ape_prof").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Edad).HasColumnName("edad");
                entity.Property(e => e.Correo).HasColumnName("correo").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Grado)
                    .HasColumnName("grado")
                    .HasConversion<string>()
                    .IsRequired();
            });

            // ==================== MEDICAMENTOS ====================
            modelBuilder.Entity<Medicamento>(entity =>
            {
                entity.ToTable("medicamentos");
                entity.HasKey(e => e.IdMedica);
                entity.Property(e => e.IdMedica).HasColumnName("id_medica");
                entity.Property(e => e.NomMedica).HasColumnName("nom_medica").HasMaxLength(200).IsRequired();
                entity.Property(e => e.Gramos).HasColumnName("gramos").IsRequired();
                entity.Property(e => e.Stock).HasColumnName("stock").IsRequired();
                entity.Property(e => e.Estado).HasColumnName("estado").IsRequired();
                entity.Property(e => e.Precio).HasColumnName("precio").IsRequired();
            });

            // ==================== MASCOTAS ====================
            modelBuilder.Entity<Mascota>(entity =>
            {
                entity.ToTable("mascotas");
                entity.HasKey(e => e.IdMascota);

                entity.Property(e => e.IdMascota).HasColumnName("id_mascota");
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente").IsRequired();
                entity.Property(e => e.NombreMasc).HasColumnName("nombre_masc").HasMaxLength(100).IsRequired();
                entity.Property(e => e.Especie).HasColumnName("especie").HasMaxLength(50).IsRequired();
                entity.Property(e => e.Raza).HasColumnName("raza").HasMaxLength(100);
                entity.Property(e => e.Edad).HasColumnName("edad");

                // Nuevos campos añadidos
                entity.Property(e => e.Sexo).HasColumnName("sexo").HasMaxLength(10);
                entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(200);
                entity.Property(e => e.Tamano).HasColumnName("tamano").HasMaxLength(20);
                entity.Property(e => e.TipoPelo).HasColumnName("tipo_pelo").HasMaxLength(20);
                entity.Property(e => e.Vacunado).HasColumnName("vacunado").HasDefaultValue(0);
                entity.Property(e => e.Notas).HasColumnName("notas").HasColumnType("text");

                // Relación con Cliente
                entity.HasOne(m => m.Cliente)
                    .WithMany(c => c.Mascotas)
                    .HasForeignKey(m => m.IdCliente)
                    .OnDelete(DeleteBehavior.Cascade);

                //Relacion con RegistroMascota 
                entity.HasMany(m => m.RegistrosMascota)
                    .WithOne(r => r.Mascota)
                    .HasForeignKey(r => r.IdMascota)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== CITAS ====================
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("citas");
                entity.HasKey(e => e.IdCita);
                entity.Property(e => e.IdCita).HasColumnName("id_cita");
                entity.Property(e => e.IdCliente).HasColumnName("id_cliente").IsRequired();
                entity.Property(e => e.IdProf).HasColumnName("id_prof").IsRequired();
                entity.Property(e => e.IdMascota).HasColumnName("id_mascota").IsRequired();
                entity.Property(e => e.FechaHora).HasColumnName("fecha_hora").IsRequired();
                entity.Property(e => e.Estado)
                    .HasColumnName("estado")
                    .HasConversion<string>()
                    .HasDefaultValue(EstadoCita.PENDIENTE);
                entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(255);

                // Relación con Cliente
                entity.HasOne(c => c.Cliente)
                    .WithMany()
                    .HasForeignKey(c => c.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict); // Evita múltiples cascadas

                // Relación con Profesional
                entity.HasOne(c => c.Profesional)
                    .WithMany()
                    .HasForeignKey(c => c.IdProf)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Mascota
                entity.HasOne(c => c.Mascota)
                    .WithMany()
                    .HasForeignKey(c => c.IdMascota)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==================== FACTURAS ====================
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.ToTable("facturas");
                entity.HasKey(e => e.IdFactura);
                entity.Property(e => e.IdFactura).HasColumnName("id_factura");
                entity.Property(e => e.IdCita).HasColumnName("id_cita").IsRequired();
                entity.Property(e => e.Monto)
                    .HasColumnName("monto")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();
                entity.Property(e => e.FechaEmision)
                    .HasColumnName("fecha_emision")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.EstadoPago)
                    .HasColumnName("estado_pago")
                    .HasConversion<string>()
                    .HasDefaultValue(EstadoPago.PENDIENTE);

                // Relación con Cita
                entity.HasOne(f => f.Cita)
                    .WithMany()
                    .HasForeignKey(f => f.IdCita)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ==================== REGISTROS ====================
            modelBuilder.Entity<Registro>(entity =>
            {
                entity.ToTable("registro");
                entity.HasKey(e => e.IdRegistro);
                entity.Property(e => e.IdRegistro).HasColumnName("id_registro");
                entity.Property(e => e.Descripcion).HasColumnName("descripcion").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Fecha)
                    .HasColumnName("fecha")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.TipoActividad).HasColumnName("tipo_actividad").HasMaxLength(50);
            });

            // ==================== REGISTRO MASCOTA ====================
            modelBuilder.Entity<RegistroMascota>(entity =>
            {
                entity.ToTable("registro_mascota");
                entity.HasKey(e => e.IdRegistro);
                entity.Property(e => e.IdRegistro)
                    .HasColumnName("id_registro")
                    .ValueGeneratedOnAdd(); 
                entity.Property(e => e.IdMascota)
                    .HasColumnName("id_mascota")
                    .IsRequired();
                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(255)
                    .IsRequired();
                entity.Property(e => e.FechaInicio)
                    .HasColumnName("fecha_inicio")
                    .IsRequired();
                entity.Property(e => e.FechaFinal)
                    .HasColumnName("fecha_final");
                entity.HasOne(e => e.Mascota)
                    .WithMany(m => m.RegistrosMascota)
                    .HasForeignKey(e => e.IdMascota)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // ==================== SERVICIO =================

            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.ToTable("servicios");
                entity.HasKey(e => e.IdServicio);
                entity.Property(e => e.IdServicio)
                    .HasColumnName("id_servicio")
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.NomServicio)
                    .HasColumnName("nom_servicio")
                    .IsRequired();
                entity.Property(e => e.Precio)
                    .HasColumnName("precio")
                    .IsRequired();
                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .IsRequired();
                
            });
        }

    }
}
