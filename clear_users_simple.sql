-- Script simplificado para eliminar apenas utilizadores e dados básicos
BEGIN TRANSACTION;

-- 1. Eliminar dados de compradores (se existir)
IF OBJECT_ID('Comprador', 'U') IS NOT NULL
    DELETE FROM Comprador;

-- 2. Eliminar vendedores (se existir)
IF OBJECT_ID('Vendedor', 'U') IS NOT NULL
    DELETE FROM Vendedor;

-- 3. Eliminar fotos (se existir)
IF OBJECT_ID('Foto', 'U') IS NOT NULL
    DELETE FROM Foto;

-- 4. Eliminar anúncios (se existir)
IF OBJECT_ID('Anuncio', 'U') IS NOT NULL
    DELETE FROM Anuncio;

-- 5. Eliminar carros (se existir)
IF OBJECT_ID('Carro', 'U') IS NOT NULL
    DELETE FROM Carro;

-- 6. Eliminar roles dos utilizadores
IF OBJECT_ID('AspNetUserRoles', 'U') IS NOT NULL
    DELETE FROM AspNetUserRoles;

-- 7. Eliminar claims dos utilizadores
IF OBJECT_ID('AspNetUserClaims', 'U') IS NOT NULL
    DELETE FROM AspNetUserClaims;

-- 8. Eliminar logins dos utilizadores
IF OBJECT_ID('AspNetUserLogins', 'U') IS NOT NULL
    DELETE FROM AspNetUserLogins;

-- 9. Eliminar tokens dos utilizadores
IF OBJECT_ID('AspNetUserTokens', 'U') IS NOT NULL
    DELETE FROM AspNetUserTokens;

-- 10. Finalmente eliminar todos os utilizadores
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
    DELETE FROM AspNetUsers;

COMMIT TRANSACTION;

-- Verificar resultado
PRINT 'Limpeza concluída!';
SELECT 'AspNetUsers' as Tabela, COUNT(*) as Total FROM AspNetUsers
UNION ALL
SELECT 'AspNetUserRoles', COUNT(*) FROM AspNetUserRoles
UNION ALL
SELECT 'Comprador', COUNT(*) FROM Comprador WHERE OBJECT_ID('Comprador', 'U') IS NOT NULL;
