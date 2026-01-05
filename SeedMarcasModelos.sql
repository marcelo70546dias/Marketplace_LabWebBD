-- Script para popular tabelas Marca e Modelo
-- Executar este script na base de dados MyOnlineStand

-- Limpar dados existentes (OPCIONAL - comentar se quiser manter marcas/modelos existentes)
-- DELETE FROM Modelo;
-- DELETE FROM Marca;
-- DBCC CHECKIDENT ('Marca', RESEED, 0);
-- DBCC CHECKIDENT ('Modelo', RESEED, 0);

-- Inserir Marcas
SET IDENTITY_INSERT Marca ON;

INSERT INTO Marca (ID_Marca, Nome) VALUES
(1, 'Peugeot'),
(2, 'Renault'),
(3, 'Volkswagen'),
(4, 'Mercedes-Benz'),
(5, 'BMW'),
(6, 'Audi'),
(7, 'Citroën'),
(8, 'Dacia'),
(9, 'SEAT'),
(10, 'Opel'),
(11, 'Fiat'),
(12, 'Skoda'),
(13, 'Volvo'),
(14, 'Toyota'),
(15, 'Hyundai'),
(16, 'Kia'),
(17, 'Nissan'),
(18, 'Ford'),
(19, 'Tesla'),
(20, 'Mazda');

SET IDENTITY_INSERT Marca OFF;

-- Inserir Modelos
SET IDENTITY_INSERT Modelo ON;

-- Peugeot (ID_Marca = 1)
INSERT INTO Modelo (ID_Modelo, Nome, ID_Marca) VALUES
(1, '208', 1),
(2, '2008', 1),
(3, '308', 1),
(4, '3008', 1),
(5, '5008', 1),

-- Renault (ID_Marca = 2)
(6, 'Clio', 2),
(7, 'Captur', 2),
(8, 'Megane', 2),
(9, 'Austral', 2),
(10, 'Arkana', 2),

-- Volkswagen (ID_Marca = 3)
(11, 'Polo', 3),
(12, 'Golf', 3),
(13, 'T-Roc', 3),
(14, 'Tiguan', 3),
(15, 'ID.3', 3),

-- Mercedes-Benz (ID_Marca = 4)
(16, 'Classe A', 4),
(17, 'Classe C', 4),
(18, 'Classe E', 4),
(19, 'GLA', 4),
(20, 'GLC', 4),

-- BMW (ID_Marca = 5)
(21, 'Série 1', 5),
(22, 'Série 3', 5),
(23, 'Série 5', 5),
(24, 'X1', 5),
(25, 'X3', 5),

-- Audi (ID_Marca = 6)
(26, 'A1', 6),
(27, 'A3', 6),
(28, 'A4', 6),
(29, 'Q3', 6),
(30, 'Q5', 6),

-- Citroën (ID_Marca = 7)
(31, 'C3', 7),
(32, 'C4', 7),
(33, 'C5 Aircross', 7),
(34, 'Berlingo', 7),
(35, 'Ami', 7),

-- Dacia (ID_Marca = 8)
(36, 'Sandero', 8),
(37, 'Duster', 8),
(38, 'Jogger', 8),
(39, 'Spring', 8),
(40, 'Logan', 8),

-- SEAT (ID_Marca = 9)
(41, 'Ibiza', 9),
(42, 'Leon', 9),
(43, 'Arona', 9),
(44, 'Ateca', 9),
(45, 'Tarraco', 9),

-- Opel (ID_Marca = 10)
(46, 'Corsa', 10),
(47, 'Astra', 10),
(48, 'Mokka', 10),
(49, 'Crossland', 10),
(50, 'Grandland', 10),

-- Fiat (ID_Marca = 11)
(51, '500', 11),
(52, 'Panda', 11),
(53, 'Tipo', 11),
(54, '500X', 11),
(55, '600', 11),

-- Skoda (ID_Marca = 12)
(56, 'Fabia', 12),
(57, 'Octavia', 12),
(58, 'Kamiq', 12),
(59, 'Karoq', 12),
(60, 'Kodiaq', 12),

-- Volvo (ID_Marca = 13)
(61, 'EX30', 13),
(62, 'XC40', 13),
(63, 'XC60', 13),
(64, 'V40', 13),
(65, 'V60', 13),

-- Toyota (ID_Marca = 14)
(66, 'Yaris', 14),
(67, 'Corolla', 14),
(68, 'C-HR', 14),
(69, 'RAV4', 14),
(70, 'Yaris Cross', 14),

-- Hyundai (ID_Marca = 15)
(71, 'i10', 15),
(72, 'i20', 15),
(73, 'i30', 15),
(74, 'Kauai', 15),
(75, 'Tucson', 15),

-- Kia (ID_Marca = 16)
(76, 'Picanto', 16),
(77, 'Stonic', 16),
(78, 'Ceed', 16),
(79, 'Sportage', 16),
(80, 'EV6', 16),

-- Nissan (ID_Marca = 17)
(81, 'Micra', 17),
(82, 'Juke', 17),
(83, 'Qashqai', 17),
(84, 'X-Trail', 17),
(85, 'Leaf', 17),

-- Ford (ID_Marca = 18)
(86, 'Fiesta', 18),
(87, 'Focus', 18),
(88, 'Puma', 18),
(89, 'Kuga', 18),
(90, 'Mustang Mach-E', 18),

-- Tesla (ID_Marca = 19)
(91, 'Model 3', 19),
(92, 'Model Y', 19),
(93, 'Model S', 19),
(94, 'Model X', 19),
(95, 'Cybertruck', 19),

-- Mazda (ID_Marca = 20)
(96, 'Mazda2', 20),
(97, 'Mazda3', 20),
(98, 'CX-30', 20),
(99, 'CX-5', 20),
(100, 'MX-5', 20);

SET IDENTITY_INSERT Modelo OFF;

-- Verificar inserção
SELECT COUNT(*) AS TotalMarcas FROM Marca;
SELECT COUNT(*) AS TotalModelos FROM Modelo;

-- Verificar relação Marca-Modelo
SELECT m.Nome AS Marca, COUNT(mo.ID_Modelo) AS NumeroModelos
FROM Marca m
LEFT JOIN Modelo mo ON m.ID_Marca = mo.ID_Marca
GROUP BY m.ID_Marca, m.Nome
ORDER BY m.Nome;
