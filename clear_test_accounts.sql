-- Script para eliminar todas as contas de teste
-- ATENÇÃO: Isto vai eliminar TODOS os utilizadores!

BEGIN TRANSACTION;

-- 1. Eliminar preferências de compradores
DELETE FROM Preferencia;

-- 2. Eliminar compradores
DELETE FROM Comprador;

-- 3. Eliminar vendedores (se a tabela existir e tiver dados)
-- DELETE FROM Vendedor; -- Descomentar se existir

-- 4. Eliminar histórico de reservas
DELETE FROM Historico_Reserva WHERE ID_Comprador IN (SELECT ID_Comprador FROM Comprador);

-- 5. Eliminar visitas
DELETE FROM Visita;

-- 6. Eliminar compras
DELETE FROM Compra;

-- 7. Eliminar filtros favoritos
DELETE FROM Filtro_Favorito;

-- 8. Eliminar fotos de anúncios
DELETE FROM Foto;

-- 9. Eliminar anúncios e carros
DELETE FROM Anuncio;
DELETE FROM Carro;

-- 10. Eliminar roles dos utilizadores
DELETE FROM AspNetUserRoles;

-- 11. Eliminar claims dos utilizadores
DELETE FROM AspNetUserClaims;

-- 12. Eliminar logins dos utilizadores
DELETE FROM AspNetUserLogins;

-- 13. Eliminar tokens dos utilizadores
DELETE FROM AspNetUserTokens;

-- 14. Finalmente eliminar todos os utilizadores
DELETE FROM AspNetUsers;

-- 15. Reset identity seeds (opcional - reinicia os IDs)
-- DBCC CHECKIDENT ('AspNetUsers', RESEED, 0);
-- DBCC CHECKIDENT ('Comprador', RESEED, 0);

COMMIT TRANSACTION;

-- Verificar que está tudo vazio
SELECT 'AspNetUsers' as Tabela, COUNT(*) as Total FROM AspNetUsers
UNION ALL
SELECT 'Comprador', COUNT(*) FROM Comprador
UNION ALL
SELECT 'Anuncio', COUNT(*) FROM Anuncio;
