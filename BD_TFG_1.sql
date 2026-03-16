

CREATE DATABASE salovet;
USE salovet;


-- Tabla clientes
CREATE TABLE clientes(
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nombre_cli VARCHAR(200) NOT NULL,
    ape_cli VARCHAR(100) NOT NULL,
    edad INT NOT NULL,
    correo VARCHAR(100) NOT NULL UNIQUE,
    tel VARCHAR(100)
);

-- Tabla usuarios 
CREATE TABLE usuarios(
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(250) NOT NULL UNIQUE,
    pass VARCHAR(200),
    profesional bool NOT NULL ,
    id_cliente INT,
    FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE
);

-- Tabla profesionales 
CREATE TABLE profesionales(
    id_prof INT AUTO_INCREMENT PRIMARY KEY,
    nom_prof VARCHAR(100) NOT NULL,
    ape_prof VARCHAR(100) NOT NULL,
    edad INT, 
    correo VARCHAR(100) NOT NULL UNIQUE,
    grado ENUM("VETERINARIO", "AYUDANTE", "PROFESIONAL") NOT NULL
);

-- Tabla medicamentos 
CREATE TABLE medicamentos(
    id_medica INT AUTO_INCREMENT PRIMARY KEY,
    nom_medica VARCHAR(200) NOT NULL,
    gramos FLOAT NOT NULL,
    stock INT NOT NULL,
    estado BOOL
);

-- Tabla mascotas 
CREATE TABLE mascotas(
    id_mascota INT AUTO_INCREMENT PRIMARY KEY,
    id_cliente INT NOT NULL,
    nombre_masc VARCHAR(100) NOT NULL,
    especie VARCHAR(50) NOT NULL,  -- Ej: Perro, Gato
    raza VARCHAR(100),
    edad INT,
    FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE
);

-- Nueva tabla: citas (conecta clientes, profesionales y mascotas)
CREATE TABLE citas(
    id_cita INT AUTO_INCREMENT PRIMARY KEY,
    id_cliente INT NOT NULL,
    id_prof INT NOT NULL,
    id_mascota INT NOT NULL,
    fecha_hora DATETIME NOT NULL,
    estado ENUM("PENDIENTE", "CONFIRMADA", "CANCELADA", "COMPLETADA") DEFAULT "PENDIENTE",
    descripcion VARCHAR(255),
    FOREIGN KEY (id_cliente) REFERENCES clientes(id_cliente) ON DELETE CASCADE,
    FOREIGN KEY (id_prof) REFERENCES profesionales(id_prof) ON DELETE CASCADE,
    FOREIGN KEY (id_mascota) REFERENCES mascotas(id_mascota) ON DELETE CASCADE
);

-- Nueva tabla: facturas (FK a citas)
CREATE TABLE facturas(
    id_factura INT AUTO_INCREMENT PRIMARY KEY,
    id_cita INT NOT NULL,
    monto DECIMAL(10,2) NOT NULL,
    fecha_emision DATETIME DEFAULT CURRENT_TIMESTAMP,
    estado_pago ENUM("PENDIENTE", "PAGADO") DEFAULT "PENDIENTE",
    FOREIGN KEY (id_cita) REFERENCES citas(id_cita) ON DELETE CASCADE
);

-- Tabla registro (corregida: typo en nombre, agregado AUTO_INCREMENT, y campos para más detalle)
CREATE TABLE registro(
    id_registro INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(255) NOT NULL,
    fecha DATETIME DEFAULT CURRENT_TIMESTAMP,
    tipo_actividad VARCHAR(50)  -- Ej: "Factura creada", "Registro insertado"
);

-- Triggers para registro automático
-- Trigger para facturas: Inserta en registro cada vez que se crea una factura
DELIMITER //
CREATE TRIGGER after_insert_factura
AFTER INSERT ON facturas
FOR EACH ROW
BEGIN
    INSERT INTO registro (descripcion, tipo_actividad) 
    VALUES (CONCAT('Factura creada para cita ID: ', NEW.id_cita, ', Monto: ', NEW.monto), 'Factura creada');
END;
//
DELIMITER ;


-- ==================== CLIENTES ====================
INSERT INTO clientes (nombre_cli, ape_cli, edad, correo, tel) VALUES
('Juan', 'Pérez', 35, 'juan.perez@email.com', '611223344'),
('María', 'González', 28, 'maria.gonzalez@email.com', '622334455'),
('Pedro', 'Martínez', 42, 'pedro.martinez@email.com', '633445566'),
('Ana', 'López', 31, 'ana.lopez@email.com', '644556677'),
('Carlos', 'Rodríguez', 38, 'carlos.rodriguez@email.com', '655667788'),
('Laura', 'Sánchez', 26, 'laura.sanchez@email.com', '666778899'),
('Diego', 'Fernández', 45, 'diego.fernandez@email.com', '677889900'),
('Sofia', 'García', 33, 'sofia.garcia@email.com', '688990011');

-- ==================== USUARIOS ====================
-- Usuarios normales (profesional = FALSE)
INSERT INTO usuarios (username, pass, profesional, id_cliente) VALUES
('juanp',   'pass123', FALSE, 1),
('mariag',  'pass456', FALSE, 2),
('pedrom',  'pass789', FALSE, 3),
('anal',    'pass321', FALSE, 4),
('carlosr', 'pass654', FALSE, 5);

-- ==================== USUARIOS DE PRUEBA ====================
-- Caso 1: Usuario con profesional = TRUE (tiene acceso profesional, sin cliente asociado)
INSERT INTO usuarios (username, pass, profesional, id_cliente) VALUES
('vet_roberto', 'zYOGDQKoMdRQiGSBFOqBo9hfXBLCFEkAGSdoGqxFpEw=', TRUE, NULL);

-- Caso 2: Usuario con profesional = FALSE (cliente normal)
INSERT INTO usuarios (username, pass, profesional, id_cliente) VALUES
('cliente_laura', 'clipass002', FALSE, 6);
-- ==================== PROFESIONALES ====================
INSERT INTO profesionales (nom_prof, ape_prof, edad, correo, grado) VALUES
('Roberto', 'Vázquez', 45, 'roberto.vazquez@veterinaria.com', 'VETERINARIO'),
('Carmen', 'Ruiz', 38, 'carmen.ruiz@veterinaria.com', 'VETERINARIO'),
('Luis', 'Moreno', 29, 'luis.moreno@veterinaria.com', 'AYUDANTE'),
('Isabel', 'Jiménez', 34, 'isabel.jimenez@veterinaria.com', 'PROFESIONAL'),
('Miguel', 'Torres', 31, 'miguel.torres@veterinaria.com', 'PROFESIONAL'),
('Elena', 'Ramírez', 27, 'elena.ramirez@veterinaria.com', 'AYUDANTE');

-- ==================== MEDICAMENTOS ====================
INSERT INTO medicamentos (nom_medica, gramos, stock, estado) VALUES
('Amoxicilina 500mg', 500, 150, 1),
('Meloxicam 5mg', 5, 80, 1),
('Dexametasona 4mg', 4, 45, 1),
('Metronidazol 250mg', 250, 120, 1),
('Omeprazol 20mg', 20, 200, 1),
('Ivermectina 1%', 10, 30, 1),
('Prednisolona 5mg', 5, 90, 1),
('Enrofloxacina 50mg', 50, 15, 1),
('Tramadol 50mg', 50, 25, 0),
('Cefalexina 500mg', 500, 180, 1);

-- ==================== MASCOTAS ====================
INSERT INTO mascotas (id_cliente, nombre_masc, especie, raza, edad) VALUES
-- Mascotas de Juan Pérez (id_cliente: 1)
(1, 'Max', 'Perro', 'Golden Retriever', 5),
(1, 'Luna', 'Gato', 'Siamés', 3),

-- Mascotas de María González (id_cliente: 2)
(2, 'Rocky', 'Perro', 'Bulldog Francés', 2),
(2, 'Mimi', 'Gato', 'Persa', 4),

-- Mascotas de Pedro Martínez (id_cliente: 3)
(3, 'Thor', 'Perro', 'Pastor Alemán', 6),

-- Mascotas de Ana López (id_cliente: 4)
(4, 'Bella', 'Perro', 'Labrador', 4),
(4, 'Coco', 'Gato', 'Mestizo', 2),
(4, 'Simba', 'Gato', 'Maine Coon', 5),

-- Mascotas de Carlos Rodríguez (id_cliente: 5)
(5, 'Rex', 'Perro', 'Rottweiler', 7),

-- Mascotas de Laura Sánchez (id_cliente: 6)
(6, 'Nala', 'Gato', 'Bengalí', 1),
(6, 'Toby', 'Perro', 'Beagle', 3),

-- Mascotas de Diego Fernández (id_cliente: 7)
(7, 'Chispa', 'Perro', 'Chihuahua', 8),
(7, 'Pelusa', 'Gato', 'Angora', 6),

-- Mascotas de Sofia García (id_cliente: 8)
(8, 'Duke', 'Perro', 'Dálmata', 4),
(8, 'Misha', 'Gato', 'Azul Ruso', 2);

-- ==================== CITAS ====================
INSERT INTO citas (id_cliente, id_prof, id_mascota, fecha_hora, estado, descripcion) VALUES
-- Citas pasadas (COMPLETADAS)
(1, 1, 1, '2025-01-15 10:00:00', 'COMPLETADA', 'Revisión general y vacunación'),
(2, 2, 3, '2025-01-20 11:30:00', 'COMPLETADA', 'Control de peso y dieta'),
(3, 1, 5, '2025-01-25 09:00:00', 'COMPLETADA', 'Tratamiento de dermatitis'),
(4, 3, 6, '2025-02-01 15:00:00', 'COMPLETADA', 'Vacuna antirrábica'),
(5, 2, 9, '2025-02-03 16:30:00', 'COMPLETADA', 'Chequeo dental'),

-- Citas recientes (CONFIRMADAS)
(6, 4, 10, '2025-02-08 10:00:00', 'CONFIRMADA', 'Primera consulta - cachorro'),
(7, 1, 12, '2025-02-09 12:00:00', 'CONFIRMADA', 'Control geriátrico'),

-- Citas futuras (PENDIENTES)
(8, 5, 14, '2025-02-10 09:30:00', 'PENDIENTE', 'Revisión post-operatoria'),
(1, 2, 2, '2025-02-11 14:00:00', 'PENDIENTE', 'Control de gatito'),
(2, 1, 4, '2025-02-12 11:00:00', 'PENDIENTE', 'Vacunación trimestral'),
(4, 3, 7, '2025-02-13 16:00:00', 'PENDIENTE', 'Desparasitación'),
(6, 2, 11, '2025-02-14 10:30:00', 'PENDIENTE', 'Control mensual'),

-- Cita cancelada
(3, 4, 5, '2025-02-05 13:00:00', 'CANCELADA', 'Cliente canceló por viaje');

-- ==================== FACTURAS ====================
-- Solo para citas COMPLETADAS
INSERT INTO facturas (id_cita, monto, fecha_emision, estado_pago) VALUES
(1, 45.50, '2025-01-15 10:45:00', 'PAGADO'),
(2, 35.00, '2025-01-20 12:00:00', 'PAGADO'),
(3, 68.75, '2025-01-25 10:30:00', 'PAGADO'),
(4, 25.00, '2025-02-01 15:30:00', 'PENDIENTE'),
(5, 120.00, '2025-02-03 17:15:00', 'PENDIENTE');

-- ==================== REGISTROS MANUALES ====================
-- Algunos registros adicionales (los triggers ya crearán automáticamente al insertar facturas)
INSERT INTO registro (descripcion, tipo_actividad, fecha) VALUES
('Sistema iniciado', 'Sistema', '2025-01-01 08:00:00'),
('Base de datos creada', 'Sistema', '2025-01-01 08:05:00'),
('Backup realizado', 'Mantenimiento', '2025-01-15 23:00:00'),
('Actualización de precios de medicamentos', 'Inventario', '2025-02-01 09:00:00'),
('Nuevo profesional agregado', 'Personal', '2025-02-05 14:30:00'),
('Backup realizado', 'Mantenimiento', '2025-02-08 23:00:00');

Select * from registro;

select * from clientes;

select * from usuarios;

UPDATE usuarios 
SET pass = 'ACtP/oqoMMgItT3YZ93EBu5EjLsRLVy/sGwit+g8SkE=' 
WHERE username = 'vet_roberto';

SELECT version();