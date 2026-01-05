-- Script para eliminar todas as contas de teste
BEGIN TRANSACTION;

-- 1. Eliminar dados de compradores
DELETE FROM Comprador;

-- 2. Eliminar roles dos utilizadores
DELETE FROM AspNetUserRoles;

-- 3. Eliminar claims dos utilizadores
DELETE FROM AspNetUserClaims;

-- 4. Eliminar logins dos utilizadores
DELETE FROM AspNetUserLogins;

-- 5. Eliminar tokens dos utilizadores
DELETE FROM AspNetUserTokens;

-- 6. Eliminar todos os utilizadores
DELETE FROM AspNetUsers;

COMMIT TRANSACTION;

-- Verificar resultado
SELECT 'AspNetUsers' as Tabela, COUNT(*) as Total FROM AspNetUsers
UNION ALL
SELECT 'AspNetUserRoles', COUNT(*) FROM AspNetUserRoles
UNION ALL
SELECT 'Comprador', COUNT(*) FROM Comprador;
