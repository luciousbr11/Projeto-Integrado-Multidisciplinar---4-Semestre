-- Script para criar usuário de teste
-- ATENÇÃO: Senhas em texto simples (sem hash)
-- Senha: 123456

-- Verificar se o usuário já existe
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'admin@teste.com')
BEGIN
    INSERT INTO Usuarios (Nome, Email, Senha, Tipo, DataCadastro)
    VALUES (
        'Administrador Teste',
        'admin@teste.com',
        '123456', -- Senha em texto simples
        'Administrador',
        GETDATE()
    );
    PRINT 'Usuário admin@teste.com criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Usuário admin@teste.com já existe.';
END

-- Verificar se o usuário de suporte já existe
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'suporte@teste.com')
BEGIN
    INSERT INTO Usuarios (Nome, Email, Senha, Tipo, DataCadastro)
    VALUES (
        'Suporte Teste',
        'suporte@teste.com',
        '123456', -- Senha em texto simples
        'Suporte',
        GETDATE()
    );
    PRINT 'Usuário suporte@teste.com criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Usuário suporte@teste.com já existe.';
END

-- Verificar se o usuário cliente já existe
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Email = 'cliente@teste.com')
BEGIN
    INSERT INTO Usuarios (Nome, Email, Senha, Tipo, DataCadastro)
    VALUES (
        'Cliente Teste',
        'cliente@teste.com',
        '123456', -- Senha em texto simples
        'Cliente',
        GETDATE()
    );
    PRINT 'Usuário cliente@teste.com criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Usuário cliente@teste.com já existe.';
END

-- Listar todos os usuários
SELECT Id, Nome, Email, Tipo, DataCadastro FROM Usuarios;
