-- Fix: Add migration to history so EF Core knows it was already applied
-- Run this in SQL Server Management Studio or: sqlcmd -S "(localdb)\MSSQLLocalDB" -d Taskify_DB -i FixMigrationHistory.sql

USE Taskify_DB;
GO

-- Add the first migration to history (if not already there)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20260113125146_jjjjj')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260113125146_jjjjj', N'8.0.22');
END
GO
