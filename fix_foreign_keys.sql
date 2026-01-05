-- Script para corrigir FKs de Utilizador para AspNetUsers
BEGIN TRANSACTION;

-- 1. DROP FK da tabela Comprador
ALTER TABLE Comprador DROP CONSTRAINT FK__Comprador__ID_Ut__59FA5E80;

-- 2. DROP FKs de outras tabelas que referenciam Utilizador
-- Vendedor
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name LIKE 'FK__Vendedor__ID_Uti%')
BEGIN
    DECLARE @VendedorFK NVARCHAR(255);
    SELECT @VendedorFK = name FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('Vendedor') 
    AND referenced_object_id = OBJECT_ID('Utilizador');
    
    IF @VendedorFK IS NOT NULL
        EXEC('ALTER TABLE Vendedor DROP CONSTRAINT ' + @VendedorFK);
END

-- Admin
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name LIKE 'FK__Admin__ID_Utiliz%')
BEGIN
    DECLARE @AdminFK NVARCHAR(255);
    SELECT @AdminFK = name FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('Admin') 
    AND referenced_object_id = OBJECT_ID('Utilizador');
    
    IF @AdminFK IS NOT NULL
        EXEC('ALTER TABLE Admin DROP CONSTRAINT ' + @AdminFK);
END

-- Preferencias
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE parent_object_id = OBJECT_ID('Preferencias'))
BEGIN
    DECLARE @PreferenciasFK NVARCHAR(255);
    SELECT @PreferenciasFK = name FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('Preferencias') 
    AND referenced_object_id = OBJECT_ID('Utilizador');
    
    IF @PreferenciasFK IS NOT NULL
        EXEC('ALTER TABLE Preferencias DROP CONSTRAINT ' + @PreferenciasFK);
END

-- 3. Criar novas FKs para AspNetUsers
ALTER TABLE Comprador 
    ADD CONSTRAINT FK_Comprador_AspNetUsers 
    FOREIGN KEY (ID_Utilizador) REFERENCES AspNetUsers(Id);

-- Vendedor (se existir a coluna)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Vendedor') AND name = 'ID_Utilizador')
BEGIN
    ALTER TABLE Vendedor 
        ADD CONSTRAINT FK_Vendedor_AspNetUsers 
        FOREIGN KEY (ID_Utilizador) REFERENCES AspNetUsers(Id);
END

-- Admin (se existir a coluna)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Admin') AND name = 'ID_Utilizador')
BEGIN
    ALTER TABLE Admin 
        ADD CONSTRAINT FK_Admin_AspNetUsers 
        FOREIGN KEY (ID_Utilizador) REFERENCES AspNetUsers(Id);
END

-- Preferencias (se existir a coluna)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Preferencias') AND name = 'ID_Utilizador')
BEGIN
    ALTER TABLE Preferencias 
        ADD CONSTRAINT FK_Preferencias_AspNetUsers 
        FOREIGN KEY (ID_Utilizador) REFERENCES AspNetUsers(Id);
END

COMMIT TRANSACTION;

PRINT 'Foreign keys atualizadas com sucesso!';
