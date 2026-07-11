/*SE CREA LA BASE DE DATOS*/
CREATE DATABASE bdBarberia 
GO

/*SE SELECCIONA LA BASE DE DATOS CREADA*/
USE bdBarberia
GO

/*INSTRUCCION QUE PERMITE CREAR LOS DIAGRAMAS*/
ALTER AUTHORIZATION ON DATABASE::bdBarberia TO sa 
GO

/*Establece el formato de la fecha en dia/mes/año*/
SET DATEFORMAT dmy
SET LANGUAGE spanish
GO

/* Creacion de las tablas */
CREATE TABLE [dbo].[ROL](
Id_Rol  INT  NOT NULL, 
Nombre NVARCHAR(50) NOT NULL,
CONSTRAINT [PK_ROL] PRIMARY KEY CLUSTERED (Id_Rol)
)
GO

--Insertar roles basicos 
INSERT INTO ROL (Id_Rol, Nombre) VALUES (1, 'Cliente');
INSERT INTO ROL (Id_Rol, Nombre) VALUES (2, 'Barbero');
INSERT INTO ROL (Id_Rol, Nombre) VALUES (3, 'Administrador');
GO

CREATE TABLE [dbo].[USUARIO](
Id_Usuario BIGINT IDENTITY(1,1) NOT NULL, 
Nombre NVARCHAR(50) NOT NULL,
Apellido NVARCHAR(50) NOT NULL,
Correo_Electronico NVARCHAR(100) UNIQUE NOT NULL,
Telefono NVARCHAR(8) NOT NULL,
PASSWORD NVARCHAR(50) NOT NULL,
Salt NVARCHAR(255) not null,
Id_Rol INT NOT NULL,
Estado INT NULL, 
Numero_Verificacion NVARCHAR(max) NULL,
Fecha_Expiracion_PIN DATETIME NULL, 
CONSTRAINT [PK_USUARIO] PRIMARY KEY CLUSTERED (Id_Usuario),
CONSTRAINT FK_Usuario_Rol FOREIGN KEY (Id_Rol) REFERENCES ROL(Id_Rol)
)
GO

CREATE TABLE [dbo].[SERVICIO](
Id_Servicio INT IDENTITY(1,1) NOT NULL, 
Nombre NVARCHAR(50)NOT NULL,
Descripcion nvarchar(200) NULL,
Duracion_Minutos INT NOT NULL, 
Precio DECIMAL(8, 2)NOT NULL,
Img_Servicio varbinary(MAX) NULL,
CONSTRAINT [PK_SERVICIO] PRIMARY KEY CLUSTERED (Id_Servicio)
)
GO

CREATE TABLE [dbo].[PRODUCTO](
Id_Producto INT IDENTITY(1,1) NOT NULL, 
Nombre NVARCHAR(50)NOT NULL,
Descripcion nvarchar(200) NULL,
Precio DECIMAL(8,2)NOT NULL, 
Stock INT NOT NULL,
Img_Producto varbinary(MAX) NULL,  
CONSTRAINT [PK_PRODUCTO] PRIMARY KEY CLUSTERED (Id_PRODUCTO)
)
GO

CREATE TABLE [dbo].[CITA] (
Id_Cita BIGINT IDENTITY(1,1) NOT NULL,
Id_Usuario BIGINT NOT NULL,
Id_Barbero BIGINT NOT NULL, 
Id_Servicio INT NOT NULL,
Fecha_Hora DATETIME NOT NULL,
Estado NVARCHAR(50) NULL, 
CONSTRAINT [PK_CITA] PRIMARY KEY CLUSTERED (Id_Cita),
CONSTRAINT FK_Cita_Barbero FOREIGN KEY (Id_Barbero) REFERENCES USUARIO(Id_Usuario),
CONSTRAINT FK_Cita_Usuario FOREIGN KEY (Id_Usuario) REFERENCES USUARIO(Id_Usuario),
CONSTRAINT FK_Cita_Servicio FOREIGN KEY (Id_Servicio) REFERENCES SERVICIO(Id_Servicio)
)
GO

CREATE TABLE FACTURA (
Id_Factura BIGINT IDENTITY(1,1) NOT NULL,
Id_Usuario BIGINT NOT NULL,
Fecha DATETIME,
Subtotal DECIMAL(10,2),
Total DECIMAL(10,2),
Estado NVARCHAR(20),
Id_Cita BIGINT NULL,  
CONSTRAINT PK_FACTURA PRIMARY KEY CLUSTERED (Id_Factura),
CONSTRAINT FK_Factura_Usuario FOREIGN KEY (Id_Usuario) REFERENCES USUARIO(Id_Usuario),
CONSTRAINT FK_Factura_Cita FOREIGN KEY (Id_Cita) REFERENCES CITA(Id_Cita)  
)
GO

CREATE TABLE FACTURA_PRODUCTO (
Id_Factura BIGINT NOT NULL,
Id_Producto INT NOT NULL,
Cantidad INT NOT NULL,
Precio DECIMAL(10,2) NOT NULL,  
CONSTRAINT PK_FACTURA_PRODUCTO PRIMARY KEY (Id_Factura, Id_Producto),
CONSTRAINT FK_Factura_Producto_Factura FOREIGN KEY (Id_Factura) REFERENCES FACTURA(Id_Factura),
CONSTRAINT FK_Factura_Producto_Producto FOREIGN KEY (Id_Producto) REFERENCES PRODUCTO(Id_Producto)
)
GO

CREATE TABLE [dbo].[ERROR_EN_BASE_DATOS] (
Id_Error INT IDENTITY(1,1) NOT NULL,
Severidad INT NULL,                     
Store_Procesure NVARCHAR(50) NULL,         
Descripcion NVARCHAR(MAX) NULL, 
Linea INT NULL,
Fecha_Hora DATETIME NULL, 
CONSTRAINT [PK_ERROR_EN_BASE_DATOS] PRIMARY KEY CLUSTERED (Id_Error)
)
GO

CREATE TABLE [dbo].[TB_BITACORA](
Id int IDENTITY(1,1) NOT NULL,
Clase nvarchar(100) NULL,
Metodo nvarchar(100) NULL,
Tipo smallint NOT NULL,
Error_id smallint NOT NULL,
Descripcion nvarchar(max) NOT NULL,
Request	nvarchar(max) NOT NULL,
Response nvarchar(max) NOT NULL,
Fecha_registro datetime NOT NULL,
CONSTRAINT [PK_TB_BITACORA] PRIMARY KEY CLUSTERED (ID),
)
GO

CREATE TABLE [dbo].[SESION] (
Id_Sesion BIGINT IDENTITY(1,1) NOT NULL,
Sesion NVARCHAR(max) NOT NULL,
Usuario BIGINT NOT NULL,
Origen nvarchar(max)  NULL,
Fecha_Inicio datetime  NULL,
Fecha_Final	 datetime NULL,
Estado int  NULL,
FECHA_ACTUALIZACION datetime  NULL
CONSTRAINT [PK_TB_SESION] PRIMARY KEY CLUSTERED (Id_Sesion),
CONSTRAINT FK_TB_SESION_USUARIO FOREIGN KEY (Usuario) REFERENCES USUARIO(Id_Usuario)
)
GO
 

--***************************************StoredProcedure***************************************--

------------------------------------------SP_LOGIN------------------------------------------------
CREATE PROCEDURE [dbo].[SP_LOGIN] 
(
    @CORREO_ELECTRONICO NVARCHAR(100), 
    @ID_USUARIO INT OUTPUT,
    @HASH_PASSWORD NVARCHAR(MAX) OUTPUT,
    @SALT NVARCHAR(MAX) OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY     
        SET @ID_USUARIO = NULL;
        SET @HASH_PASSWORD = NULL;
        SET @SALT = NULL;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = NULL;

        -- Verificar si el correo existe
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO)
        BEGIN
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El correo electrónico no está registrado';
            RETURN;
        END

        -- Verificar si el usuario está activo
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Estado = 1)
        BEGIN
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El usuario está inactivo';
            RETURN;
        END

        -- Obtener credenciales del usuario
        SELECT 
            @ID_USUARIO = Id_Usuario,
            @HASH_PASSWORD = PASSWORD,
            @SALT = Salt
        FROM USUARIO
        WHERE Correo_Electronico = @CORREO_ELECTRONICO;

        -- Retornar información del usuario
        SELECT 
            Id_Usuario,
            Id_Rol,
            Nombre,
            Apellido,
            Correo_Electronico,
            Telefono
        FROM USUARIO
        WHERE Id_Usuario = @ID_USUARIO;
    
    END TRY
    BEGIN CATCH
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();
        
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Linea,
            Descripcion,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_InicioSesion----------------------------------------
CREATE PROCEDURE [dbo].[SP_INICIO_SESION]
(
    @SESION nvarchar(max),
	@USUARIO INT,
	@ORIGEN nvarchar(max),
	
	@IDRETURN int output,
	@ERRORID int output,
	@ERRORDESCRIPCION nvarchar(max) output
)
AS
BEGIN	
				INSERT INTO SESION
					(
						Sesion,
						Usuario,
						Origen,
						[FECHA_INICIO],
						[ESTADO],
						[FECHA_ACTUALIZACION]
					)
				VALUES
					(
						@SESION,
						@USUARIO,
						@ORIGEN,
						GETUTCDATE(),
						1,
						GETUTCDATE()
					);

					set @idReturn = scope_identity();
	
					SET @IDRETURN = @idReturn
					SET @ERRORID = 0;
					SET @ERRORDESCRIPCION = '';
	
	
	
END
GO
------------------------------------------SP_CerrarSesion-----------------------------------------
CREATE PROCEDURE [dbo].[SP_CERRAR_SESION]
(
	@SESION nvarchar(max)
)
AS
BEGIN
				UPDATE SESION 
					SET
					ESTADO = 0,
					FECHA_FINAL = GETUTCDATE(),
					FECHA_ACTUALIZACION= GETUTCDATE()
				WHERE
					SESION = @SESION

END
GO
------------------------------------------SP_ActivarUsuario----------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTIVAR_CUENTA]
(
    @CORREO_ELECTRONICO NVARCHAR(100),
    @NUMERORVERIFICACION NVARCHAR(MAX),
    @FECHA_ACTUAL DATETIME,
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT,
    @FILASACTUALIZADAS INT OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el correo existe
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El correo electrónico no está registrado.';
            RETURN;
        END

        -- Verificar si el usuario ya está activo
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'El usuario ya está activado.';
            RETURN;
        END

        -- Verificar si el PIN es incorrecto
        IF NOT EXISTS (
            SELECT 1 FROM USUARIO 
            WHERE Correo_Electronico = @CORREO_ELECTRONICO 
            AND Numero_Verificacion = @NUMERORVERIFICACION
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 3; 
            SET @ERRORDESCRIPCION = 'El número de verificación es incorrecto.';
            RETURN;
        END

        -- Verificar si el PIN ha expirado
        IF EXISTS (
            SELECT 1 FROM USUARIO 
            WHERE Correo_Electronico = @CORREO_ELECTRONICO 
            AND Numero_Verificacion = @NUMERORVERIFICACION
            AND Fecha_Expiracion_PIN < @FECHA_ACTUAL 
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 4; 
            SET @ERRORDESCRIPCION = 'El número de verificación ha expirado.';
            RETURN;
        END

        UPDATE USUARIO 
        SET Estado = 1 
        WHERE Correo_Electronico = @CORREO_ELECTRONICO 
        AND Numero_Verificacion = @NUMERORVERIFICACION 
        AND Estado = 0;

        SET @FILASACTUALIZADAS = @@ROWCOUNT;

        IF @FILASACTUALIZADAS > 0
        BEGIN
            SET @IDRETURN = 1; 
            SET @ERRORID = NULL;
            SET @ERRORDESCRIPCION = NULL;
        END
        ELSE
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 5; 
            SET @ERRORDESCRIPCION = 'No se pudo actualizar el estado del usuario.';
        END
    END TRY

    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Linea,
            Descripcion,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            @FECHA_ACTUAL 
        );
    END CATCH
END
GO
------------------------------------------SP_SolicitarNuevo codigo----------------------------------------
CREATE PROCEDURE [dbo].[SP_SOLICITAR_NUEVO_CODIGO]
(
    @CORREO_ELECTRONICO NVARCHAR(100),
    @NUMERORVERIFICACION NVARCHAR(MAX),
    @FECHA_ACTUAL DATETIME,
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el correo electrónico existe en la base de datos
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El correo electrónico no está registrado';
            RETURN;
        END

        -- Verificar si el usuario está activado (Estado = 1)
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El usuario ya está activado y no necesita un nuevo código de verificación.';
            RETURN;
        END

        -- Actualizar el número de verificación con el nuevo código proporcionado desde el backend
        UPDATE USUARIO 
        SET Numero_Verificacion = @NUMERORVERIFICACION,
            Fecha_Expiracion_PIN = DATEADD(MINUTE, 5, @FECHA_ACTUAL)
        WHERE Correo_Electronico = @CORREO_ELECTRONICO 
        AND Estado = 0;

        -- Verificar si la actualización fue exitosa
        IF @@ROWCOUNT > 0
        BEGIN
            SET @IDRETURN = 1;  -- Éxito
            SET @ERRORID = NULL;
            SET @ERRORDESCRIPCION = 'Nuevo código de verificación generado exitosamente.';
        END
        ELSE
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'No se pudo generar el nuevo código de verificación.';
        END
    END TRY
    BEGIN CATCH

        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Linea,
            Descripcion,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_CODIGO_CAMBIO_CONTRA--------------------------------------
CREATE PROCEDURE [dbo].[SP_CODIGO_CAMBIO_CONTRA]
(
    @CORREO_ELECTRONICO NVARCHAR(100),
    @NUMERORVERIFICACION NVARCHAR(MAX), 
    @FECHA_ACTUAL DATETIME,
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el correo electrónico existe y está activado (Estado = 1)
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El correo electrónico no está registrado o la cuenta no está activada.';
            RETURN;
        END
        -- Actualizar el número de verificación con el nuevo código
        UPDATE USUARIO 
        SET Numero_Verificacion = @NUMERORVERIFICACION,
            Fecha_Expiracion_PIN = DATEADD(MINUTE, 5, @FECHA_ACTUAL) 
        WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Estado = 1;

        -- Verificar si la actualización fue exitosa
        IF @@ROWCOUNT > 0
        BEGIN
            SET @IDRETURN = 1;  
            SET @ERRORID = NULL;
            SET @ERRORDESCRIPCION = 'Código de verificación para cambio de contraseña generado exitosamente.';
        END
        ELSE
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'No se pudo generar el código de verificación. Intente de nuevo.';
        END
    END TRY
    BEGIN CATCH
        -- Capturar errores de SQL Server
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar el error en la tabla de logs
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Linea,
            Descripcion,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_CambiarContraseña----------------------------------------
CREATE PROCEDURE [dbo].[SP_CAMBIAR_CONTRASEÑA]
(
    @CORREO_ELECTRONICO NVARCHAR(100),
    @NUMERORVERIFICACION NVARCHAR(MAX), 
    @NUEVO_PASSWORD NVARCHAR(MAX),
    @NUEVO_SALT NVARCHAR(MAX),
    @FECHA_ACTUAL DATETIME, 
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el usuario existe y está activo
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; -- Código de error personalizado
            SET @ERRORDESCRIPCION = 'Usuario no encontrado o inactivo';
            RETURN;
        END

       -- Verificar si el PIN es incorrecto
        IF NOT EXISTS (
            SELECT 1 FROM USUARIO 
            WHERE Correo_Electronico = @CORREO_ELECTRONICO 
            AND Numero_Verificacion = @NUMERORVERIFICACION
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El número de verificación es incorrecto.';
            RETURN;
        END

        -- Verificar si el PIN ha expirado
        IF EXISTS (
            SELECT 1 FROM USUARIO 
            WHERE Correo_Electronico = @CORREO_ELECTRONICO 
            AND Numero_Verificacion = @NUMERORVERIFICACION
            AND Fecha_Expiracion_PIN < @FECHA_ACTUAL 
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El número de verificación ha expirado.';
            RETURN;
        END

        -- Actualizar la contraseña y el salt
        UPDATE USUARIO
        SET 
            PASSWORD = @NUEVO_PASSWORD,
            Salt = @NUEVO_SALT,
            Numero_Verificacion = NULL
        WHERE 
            Correo_Electronico = @CORREO_ELECTRONICO;

        -- Confirmar éxito
        SET @IDRETURN = 1;
        SET @ERRORID = NULL;
        SET @ERRORDESCRIPCION = 'Contraseña actualizada correctamente';

    END TRY
    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_IngresarUsuario---------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_USUARIO]
(
   @NOMBRE NVARCHAR(50),
   @APELLIDO NVARCHAR(50),
   @CORREO_ELECTRONICO NVARCHAR(100),
   @TELEFONO NVARCHAR(9),
   @PASSWORD NVARCHAR(50),
   @SALT NVARCHAR(255), 
   @NUMERORVERIFICACION NVARCHAR(MAX),
   @FECHA_ACTUAL DATETIME, 

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el correo ya está registrado
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'Correo ya registrado';
            RETURN;
        END

        INSERT INTO USUARIO 
        (
            Nombre,
            Apellido,
            Correo_Electronico,
            Telefono,
            PASSWORD,
            Salt, -- revisar
            Id_Rol,       
            Numero_Verificacion, 
            Fecha_Expiracion_PIN, 
            Estado           
        )
        VALUES
        (
            @NOMBRE,
            @APELLIDO,
            @CORREO_ELECTRONICO,
            @TELEFONO,
            @PASSWORD,
            @SALT, 
            1, 
            @NUMERORVERIFICACION,
            DATEADD(MINUTE, 5, @FECHA_ACTUAL), 
            0 
        );

        SET @IDRETURN = SCOPE_IDENTITY();
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Inserción exitosa, código de verificación generado y expira en 5 minutos';

    END TRY

    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_ActualizarUsuario--------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTUALIZAR_USUARIO]
( 
   @ID_USUARIO BIGINT,       
   @NOMBRE NVARCHAR(50),
   @APELLIDO NVARCHAR(50),
   @CORREO_ELECTRONICO NVARCHAR(100),
   @TELEFONO NVARCHAR(9),
   @IDRETURN int output,
   @ERRORID int output,
   @ERRORDESCRIPCION nvarchar(max) output
)
AS 
BEGIN 
    BEGIN TRY 
        -- Verificar que el usuario exista y que esté activo
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO  AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El Usuario No Existe O Inactivo';
            RETURN;
        END

        -- Verificar si el correo ya está en uso por otro usuario
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Id_Usuario <> @ID_USUARIO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;  
            SET @ERRORDESCRIPCION = 'Correo ya registrado por otro usuario';
            RETURN;
        END

        UPDATE USUARIO
        SET 
            Nombre = @NOMBRE,
            Apellido = @APELLIDO,
            Correo_Electronico = @CORREO_ELECTRONICO,
            Telefono = @TELEFONO
        WHERE Id_Usuario = @ID_USUARIO;

        -- Devolver éxito
        SET @IDRETURN = @ID_USUARIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Actualización exitosa';

    END TRY
    BEGIN CATCH

        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_EliminarUsuario----------------------------------------
CREATE PROCEDURE [dbo].[SP_ELIMINAR_USUARIO]
( 
   @ID_USUARIO BIGINT,       
   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS 
BEGIN 
    DECLARE @ROL INT;

    BEGIN TRY 
        -- Verificar si el usuario existe
        SELECT @ROL = Id_Rol FROM USUARIO WHERE Id_Usuario = @ID_USUARIO;

        IF @ROL IS NULL
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El usuario no existe';
            RETURN;
        END

        -- Validaciones según el rol del usuario
        IF @ROL = 1 
        BEGIN
            IF EXISTS (SELECT 1 FROM CITA WHERE Id_Usuario = @ID_USUARIO AND Estado = 'Pendiente')
            BEGIN
                SET @IDRETURN = -1;
                SET @ERRORID = 2;
                SET @ERRORDESCRIPCION = 'El cliente tiene citas pendientes.';
                RETURN;
            END

            IF EXISTS (SELECT 1 FROM FACTURA WHERE Id_Usuario = @ID_USUARIO AND Estado = 'Pendiente')
            BEGIN
                SET @IDRETURN = -1;
                SET @ERRORID = 3;
                SET @ERRORDESCRIPCION = 'El cliente tiene facturas pendientes.';
                RETURN;
            END

            IF EXISTS (SELECT 1 FROM CITA WHERE Id_Usuario = @ID_USUARIO AND Estado = 'No asistió')
            BEGIN
                SET @IDRETURN = -1;
                SET @ERRORID = 4;
                SET @ERRORDESCRIPCION = 'No se puede eliminar la cuenta. Tiene citas a las que no asistió. Por favor, comuníquese con la barbería.';
                RETURN;
            END

            -- Eliminar citas completadas o canceladas
            DELETE FROM CITA WHERE Id_Usuario = @ID_USUARIO AND Estado NOT IN ('Pendiente', 'No asistió');

            -- Eliminar facturas no pendientes
            DELETE FROM FACTURA WHERE Id_Usuario = @ID_USUARIO AND Estado <> 'Pendiente';
        END
        ELSE IF @ROL = 2 
        BEGIN
            IF EXISTS (SELECT 1 FROM CITA WHERE Id_Barbero = @ID_USUARIO AND Estado = 'Pendiente')
            BEGIN
                SET @IDRETURN = -1;
                SET @ERRORID = 5;
                SET @ERRORDESCRIPCION = 'El barbero tiene citas pendientes.';
                RETURN;
            END

            -- Eliminar citas completadas/canceladas como barbero
            DELETE FROM CITA WHERE Id_Barbero = @ID_USUARIO AND Estado <> 'Pendiente';
        END
        -- Administrador (Id_Rol = 3): No se eliminan restricciones específicas

        -- Eliminar cualquier sesión activa
        DELETE FROM SESION WHERE Usuario = @ID_USUARIO;

        -- Finalmente eliminar el usuario
        DELETE FROM USUARIO WHERE Id_Usuario = @ID_USUARIO;

        SET @IDRETURN = @ID_USUARIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Usuario eliminado correctamente';

    END TRY
    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_OBTENERUsuario----------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_USUARIO]
(
	@ID_USUARIO BIGINT
)
AS
BEGIN
   
        SELECT 
		    USUARIO.Id_Usuario,
			USUARIO.Nombre,
            USUARIO.Apellido,
            USUARIO.Correo_Electronico,
            USUARIO.Telefono,    
			USUARIO.Id_Rol
        FROM 
            USUARIO
		WHERE
			USUARIO.Id_Usuario = @ID_USUARIO and USUARIO.Id_Rol = 1;
END
GO
------------------------------------------SP_OBTENER_LISTAUSUARIOS--------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_LISTAUSUARIOS]
AS
BEGIN
   
        SELECT 
		    USUARIO.Id_Usuario,
			USUARIO.Nombre,
            USUARIO.Apellido,
            USUARIO.Correo_Electronico,
            USUARIO.Telefono,
		    USUARIO.Id_Rol
        FROM 
           USUARIO
		WHERE USUARIO.Id_Rol = 1;
END
GO


------------------------------------------SP_IngresarBarbero---------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_BARBERO]
(
   @NOMBRE NVARCHAR(50),
   @APELLIDO NVARCHAR(50),
   @CORREO_ELECTRONICO NVARCHAR(100),
   @TELEFONO NVARCHAR(9),
   @PASSWORD NVARCHAR(50),
   @SALT NVARCHAR(255), 
   @NUMERORVERIFICACION NVARCHAR(MAX),
   @FECHA_ACTUAL DATETIME, 

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el correo ya está registrado
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'Correo ya registrado';
            RETURN;
        END

        INSERT INTO USUARIO 
        (
            Nombre,
            Apellido,
            Correo_Electronico,
            Telefono,
            PASSWORD,
            Salt, 
            Id_Rol,       
            Numero_Verificacion, 
            Fecha_Expiracion_PIN, 
            Estado           
        )
        VALUES
        (
            @NOMBRE,
            @APELLIDO,
            @CORREO_ELECTRONICO,
            @TELEFONO,
            @PASSWORD,
            @SALT, 
            2, 
            @NUMERORVERIFICACION,
            DATEADD(MINUTE, 5, @FECHA_ACTUAL), 
            0 
        );

        -- Obtener el ID del usuario insertado
        SET @IDRETURN = SCOPE_IDENTITY();
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Inserción exitosa, código de verificación generado y expira en 5 minutos';

    END TRY

    BEGIN CATCH
        -- Capturar el número y mensaje de error
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar el error en la tabla de logs
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_ActualizarBarbero---------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTUALIZAR_BARBERO]
( 
   @ID_USUARIO BIGINT,       
   @NOMBRE NVARCHAR(50),
   @APELLIDO NVARCHAR(50),
   @CORREO_ELECTRONICO NVARCHAR(100),
   @TELEFONO NVARCHAR(9),
   @PASSWORD NVARCHAR(50),
   @SALT NVARCHAR(255),

   @IDRETURN int output,
   @ERRORID int output,
   @ERRORDESCRIPCION nvarchar(max) output
)
AS 
BEGIN 
    BEGIN TRY 
	     IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 2 AND Estado = 1)
		 BEGIN
		    SET @IDRETURN = -1;
			SET @ERRORID = 1; 
			SET @ERRORDESCRIPCION = 'El usuario no existe o inactivo';
			RETURN;
	END

	    -- Verificar si el correo ya está en uso por otro usuario
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Id_Usuario <> @ID_USUARIO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;  
            SET @ERRORDESCRIPCION = 'Correo ya registrado por otro usuario';
            RETURN;
        END

        UPDATE USUARIO
        SET 
            Nombre = @NOMBRE,
            Apellido = @APELLIDO,
            Correo_Electronico = @CORREO_ELECTRONICO,
            Telefono = @TELEFONO,
            PASSWORD = @PASSWORD,   
            Salt = @SALT            
            WHERE Id_Usuario = @ID_USUARIO;

        SET @IDRETURN = @ID_USUARIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Actualización exitosa';

    END TRY
    BEGIN CATCH
        -- Capturar error
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_EliminarBarbero---------------------------------------
CREATE PROCEDURE [dbo].[SP_ELIMINAR_BARBERO]
( 
   @ID_USUARIO BIGINT,       

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS 
BEGIN 
    BEGIN TRY 
        -- Verificar si el usuario existe y es barbero
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 2)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El barbero no existe.';
            RETURN;
        END

        -- Validar que el barbero no tenga citas pendientes
        IF EXISTS (
            SELECT 1 
            FROM CITA 
            WHERE Id_Barbero = @ID_USUARIO AND Estado = 'Pendiente'
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2; 
            SET @ERRORDESCRIPCION = 'El barbero tiene citas pendientes y no puede ser eliminado.';
            RETURN;
        END

        -- Eliminar barbero
        DELETE FROM USUARIO WHERE Id_Usuario = @ID_USUARIO;

        -- Confirmar eliminación
        SET @IDRETURN = @ID_USUARIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Barbero eliminado correctamente.';

    END TRY
    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_OBTENER_LISTAUSUARIOS--------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_LISTABARBEROS]
AS
BEGIN
   
        SELECT 
	        USUARIO.Id_Usuario,
			USUARIO.Nombre,
            USUARIO.Apellido,
            USUARIO.Correo_Electronico,
            USUARIO.Telefono,
			USUARIO.Id_Rol
        FROM 
           USUARIO
		WHERE USUARIO.Id_Rol = 2;
END
GO


------------------------------------------SP_IngresarAdmin---------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_ADMIN]
(
   @NOMBRE NVARCHAR(50),
   @APELLIDO NVARCHAR(50),
   @CORREO_ELECTRONICO NVARCHAR(100),
   @TELEFONO NVARCHAR(9),
   @PASSWORD NVARCHAR(50),
   @SALT NVARCHAR(255), 
   @NUMERORVERIFICACION NVARCHAR(MAX),
   @FECHA_ACTUAL DATETIME, 

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el correo ya está registrado
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'Correo ya registrado';
            RETURN;
        END

        -- Insertar el usuario con la fecha de expiración en 5 minutos
        INSERT INTO USUARIO 
        (
            Nombre,
            Apellido,
            Correo_Electronico,
            Telefono,
            PASSWORD,
            Salt, 
            Id_Rol,       
            Numero_Verificacion, 
            Fecha_Expiracion_PIN, 
            Estado           
        )
        VALUES
        (
            @NOMBRE,
            @APELLIDO,
            @CORREO_ELECTRONICO,
            @TELEFONO,
            @PASSWORD,
            @SALT, 
            3, 
            @NUMERORVERIFICACION,
            DATEADD(MINUTE, 5, @FECHA_ACTUAL), 
            0 
        );

        -- Obtener el ID del usuario insertado
        SET @IDRETURN = SCOPE_IDENTITY();
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Inserción exitosa, código de verificación generado y expira en 5 minutos';

    END TRY

    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_ActualizarAdmin--------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTUALIZAR_ADMIN]
( 
   @ID_USUARIO BIGINT,       
   @NOMBRE NVARCHAR(50),
   @APELLIDO NVARCHAR(50),
   @CORREO_ELECTRONICO NVARCHAR(100),
   @TELEFONO NVARCHAR(9),
   @PASSWORD NVARCHAR(50),
   @SALT NVARCHAR(255), 

   @IDRETURN int output,
   @ERRORID int output,
   @ERRORDESCRIPCION nvarchar(max) output
)
AS 
BEGIN 
    BEGIN TRY 
	     IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 3 AND Estado = 1)
		 BEGIN
		    SET @IDRETURN = -1;
			SET @ERRORID = 1; 
			SET @ERRORDESCRIPCION = 'El usuario no existe o inactivo';
			RETURN;
	END

	    -- Verificar si el correo ya está en uso por otro usuario
        IF EXISTS (SELECT 1 FROM USUARIO WHERE Correo_Electronico = @CORREO_ELECTRONICO AND Id_Usuario <> @ID_USUARIO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;  
            SET @ERRORDESCRIPCION = 'Correo ya registrado por otro usuario';
            RETURN;
        END

        UPDATE USUARIO
        SET 
            Nombre = @NOMBRE,
            Apellido = @APELLIDO,
            Correo_Electronico = @CORREO_ELECTRONICO,
            Telefono = @TELEFONO,
            PASSWORD = @PASSWORD,   
            Salt = @SALT           
            WHERE Id_Usuario = @ID_USUARIO;

        SET @IDRETURN = @ID_USUARIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Actualización exitosa';

    END TRY
    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_EliminarAdmin----------------------------------------
CREATE PROCEDURE [dbo].[SP_ELIMINAR_ADMIN]
( 
   @ID_USUARIO BIGINT,       

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS 
BEGIN 
    BEGIN TRY 
        -- Verificar si el usuario existe y es un cliente
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 3)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'El usuario no existe o no es un Admin.';
            RETURN;
        END

        DELETE FROM USUARIO WHERE Id_Usuario = @ID_USUARIO;

        SET @IDRETURN = @ID_USUARIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Usuario eliminado correctamente';

    END TRY
    BEGIN CATCH
        -- Capturar error
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO

------------------------------------------SP_OBTENER_LISTAADMIN--------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_LISTAADMIN]
AS
BEGIN
   
        SELECT 
	        USUARIO.Id_Usuario,
			USUARIO.Nombre,
            USUARIO.Apellido,
            USUARIO.Correo_Electronico,
            USUARIO.Telefono,
			USUARIO.Id_Rol
        FROM 
           USUARIO
		WHERE USUARIO.Id_Rol = 3;
END
GO

------------------------------------------SP_InsertarServicio--------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_SERVICIO]
(
   @NOMBRE NVARCHAR(50),
   @DESCRIPCION NVARCHAR(200),
   @DURACION_MINUTOS INT,
   @PRECIO DECIMAL(8, 2),
   
   @IDRETURN int output,
   @ERRORID int output,
   @ERRORDESCRIPCION nvarchar(max) output
)
AS
BEGIN
   BEGIN TRY
        INSERT INTO SERVICIO
        (
            Nombre,
            Descripcion,
            Duracion_Minutos,
            Precio
        )
        VALUES
        (
            @Nombre,
            @Descripcion,
            @Duracion_Minutos,
            @Precio
        );

        SET @IDRETURN = SCOPE_IDENTITY();
        SET @ERRORID = 0; 
        SET @ERRORDESCRIPCION = 'Servicio creado exitosamente';
    END TRY

    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
			Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
			ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_ActualizarServicio-------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTUALIZAR_SERVICIO] 
(
   @ID_SERVICIO INT,
   @NOMBRE NVARCHAR(50),
   @DESCRIPCION NVARCHAR(200),
   @DURACION_MINUTOS INT,
   @PRECIO DECIMAL(8, 2),
   
   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el servicio existe
        IF NOT EXISTS (SELECT 1 FROM SERVICIO WHERE Id_Servicio = @ID_SERVICIO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El servicio no existe';
            RETURN;
        END

        -- Actualizar servicio
        UPDATE SERVICIO
        SET 
            Nombre = @NOMBRE,
            Descripcion = @DESCRIPCION,
            Duracion_Minutos = @DURACION_MINUTOS,
            Precio = @PRECIO
        WHERE Id_Servicio = @ID_SERVICIO;

        SET @IDRETURN = @ID_SERVICIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Servicio actualizado exitosamente';
    END TRY

    BEGIN CATCH
        -- Capturar error
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_EliminarServicio---------------------------------------
CREATE PROCEDURE [dbo].[SP_ELIMINAR_SERVICIO] 
(
   @ID_SERVICIO INT,
   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el servicio existe
        IF NOT EXISTS (SELECT 1 FROM SERVICIO WHERE Id_Servicio = @ID_SERVICIO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El servicio no existe';
            RETURN;
        END

        -- Verificar si el servicio está asociado a citas REVISAR
        IF EXISTS (SELECT 1 FROM CITA WHERE Id_Servicio = @ID_SERVICIO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'No se puede eliminar el servicio porque tiene citas asociadas';
            RETURN;
        END

        -- Eliminar servicio
        DELETE FROM SERVICIO WHERE Id_Servicio = @ID_SERVICIO;

        -- Confirmar eliminación
        SET @IDRETURN = @ID_SERVICIO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Servicio eliminado correctamente';
    END TRY

    BEGIN CATCH
        -- Capturar error
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar error en la tabla de errores
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_OBTENER_LISTASERVICIOS------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_LISTASERVICIOS]
AS
BEGIN
   
        SELECT 
		    SERVICIO.Id_Servicio,
			SERVICIO.Nombre,
            SERVICIO.Descripcion,
            SERVICIO.Duracion_Minutos,
            SERVICIO.Precio           
        FROM 
            SERVICIO
END
GO


------------------------------------------SP_InsertarProducto--------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_PRODUCTO]
(
    @NOMBRE NVARCHAR(50),
    @DESCRIPCION NVARCHAR(200),
    @PRECIO DECIMAL(8, 2),
    @STOCK INT,

    @IDRETURN int output,
    @ERRORID int output,
    @ERRORDESCRIPCION nvarchar(max) output
)
AS
BEGIN
   BEGIN TRY
         INSERT INTO PRODUCTO 
            (
                Nombre,
                Descripcion,
                Precio,
                Stock
            )
            VALUES
            (
                @Nombre,
                @Descripcion,
                @Precio,
                @Stock
            );

        SET @IDRETURN = SCOPE_IDENTITY();
        SET @ERRORID = 0; 
        SET @ERRORDESCRIPCION = 'Servicio creado exitosamente';
    END TRY

    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
			Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
			ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_ActualizarProducto------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTUALIZAR_PRODUCTO] 
(
   @ID_PRODUCTO INT,
   @NOMBRE NVARCHAR(50),
   @DESCRIPCION NVARCHAR(200),
   @PRECIO DECIMAL(8, 2),
   @STOCK INT,
   
   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el producto existe
        IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE Id_Producto = @ID_PRODUCTO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El producto no existe';
            RETURN;
        END

        UPDATE PRODUCTO
        SET 
            Nombre = @NOMBRE,
            Descripcion = @DESCRIPCION,
            Precio = @PRECIO,
            Stock = @STOCK
        WHERE Id_Producto = @ID_PRODUCTO;

        SET @IDRETURN = @ID_PRODUCTO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Producto actualizado exitosamente';
    END TRY

    BEGIN CATCH

        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();


        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_EliminarProducto--------------------------------------
CREATE PROCEDURE [dbo].[SP_ELIMINAR_PRODUCTO] 
(
   @ID_PRODUCTO INT,
   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si el producto existe
        IF NOT EXISTS (SELECT 1 FROM PRODUCTO WHERE Id_Producto = @ID_PRODUCTO)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El producto no existe';
            RETURN;
        END

		--Revisar Cuando esten las facturas
        -- Verificar si el producto está asociado a facturas
       -- IF EXISTS (SELECT 1 FROM DETALLE_FACTURA WHERE Id_Producto = @ID_PRODUCTO)
      --  BEGIN
        --    SET @IDRETURN = -1;
         --   SET @ERRORID = 2;
        --    SET @ERRORDESCRIPCION = 'No se puede eliminar el producto porque está asociado a facturas';
       --     RETURN;
      --  END

        -- Eliminar producto
        DELETE FROM PRODUCTO WHERE Id_Producto = @ID_PRODUCTO;

        -- Confirmar eliminación
        SET @IDRETURN = @ID_PRODUCTO;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Producto eliminado correctamente';
    END TRY

    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END;
GO
------------------------------------------SP_OBTENER_LISTAPRODUCTOS--------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_LISTAPRODUCTOS]
AS
BEGIN
   
        SELECT
		    PRODUCTO.Id_Producto,
            PRODUCTO.Nombre,
            PRODUCTO.Descripcion,
            PRODUCTO.Precio,
            PRODUCTO.Stock
        FROM 
            PRODUCTO
END
GO



------------------------------------------SP_InsertaCita--------------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_CITA]
(
   @ID_USUARIO BIGINT,
   @ID_BARBERO BIGINT,
   @ID_SERVICIO INT,
   @FECHA_HORA DATETIME,

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    DECLARE @DURACION INT;
    DECLARE @FECHA_FIN DATETIME;

    BEGIN TRY
        -- Verificar si la fecha de la cita es anterior a la fecha actual
        IF @FECHA_HORA < GETDATE()
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'No se pueden agendar citas en fechas pasadas.';
            RETURN;
        END

        -- Verificar si el barbero tiene el rol correcto y está activo
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_BARBERO AND Id_Rol = 2 AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'El barbero no existe o no está activo.';
            RETURN;
        END

        -- Verificar si el usuario tiene el rol de cliente y está activo
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 1 AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 3;
            SET @ERRORDESCRIPCION = 'El usuario no existe o está inactivo.';
            RETURN;
        END

        -- Verificar si el servicio existe y obtener su duración
        SELECT @DURACION = Duracion_Minutos FROM SERVICIO WHERE Id_Servicio = @ID_SERVICIO;

        IF @DURACION IS NULL
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 4;
            SET @ERRORDESCRIPCION = 'El servicio no existe.';
            RETURN;
        END

        -- Calcular la fecha de finalización de la cita
        SET @FECHA_FIN = DATEADD(MINUTE, @DURACION, @FECHA_HORA);

        -- Verificar si el barbero tiene citas en el rango de tiempo del servicio
        IF EXISTS (
            SELECT 1 FROM CITA
            WHERE Id_Barbero = @ID_BARBERO
            AND (
                (Fecha_Hora BETWEEN @FECHA_HORA AND @FECHA_FIN) -- Citas que inician dentro del rango
                OR (DATEADD(MINUTE, (SELECT Duracion_Minutos FROM SERVICIO WHERE Id_Servicio = CITA.Id_Servicio), Fecha_Hora) 
                    BETWEEN @FECHA_HORA AND @FECHA_FIN) -- Citas que terminan dentro del rango
                OR (@FECHA_HORA BETWEEN Fecha_Hora AND DATEADD(MINUTE, (SELECT Duracion_Minutos FROM SERVICIO WHERE Id_Servicio = CITA.Id_Servicio), Fecha_Hora)) -- Nueva cita dentro de una ya existente
            )
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 5;
            SET @ERRORDESCRIPCION = 'El barbero ya tiene una cita en este horario.';
            RETURN;
        END

        -- Insertar la cita
        INSERT INTO CITA 
        (
            Id_Usuario,
            Id_Barbero,
            Id_Servicio,
            Fecha_Hora,
            Estado
        )
        VALUES
        (
            @ID_USUARIO,
            @ID_BARBERO,
            @ID_SERVICIO,
            @FECHA_HORA,
            'Pendiente'
        );

        -- Obtener el ID de la cita recién insertada
        SET @IDRETURN = SCOPE_IDENTITY();
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Cita registrada exitosamente.';

        -- Devolver la información de la cita registrada
        SELECT 
            C.Id_Cita,
            C.Fecha_Hora,
            C.Estado,

            S.Id_Servicio,
            S.Nombre AS Servicio_Nombre,
            S.Duracion_Minutos AS Servicio_Duracion,
            S.Precio AS Servicio_Precio,

            B.Id_Usuario AS Id_Barbero,
            B.Nombre AS Barbero_Nombre,

            U.Id_Usuario AS Id_Usuario,
            U.Nombre AS Usuario_Nombre,
            U.Apellido AS Usuario_Apellido,
            U.Correo_Electronico AS Usuario_Correo,
            U.Telefono AS Usuario_Telefono
        FROM CITA C
        INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
        INNER JOIN USUARIO B ON C.Id_Barbero = B.Id_Usuario
        INNER JOIN USUARIO U ON C.Id_Usuario = U.Id_Usuario
        WHERE C.Id_Cita = @IDRETURN;

    END TRY
    BEGIN CATCH
        -- Manejo de errores
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar el error en la base de datos
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_ActualizarCita--------------------------------------------
CREATE PROCEDURE [dbo].[SP_ACTUALIZAR_CITA]
(
   @ID_CITA BIGINT,
   @ID_BARBERO BIGINT,
   @ID_USUARIO BIGINT,
   @ID_SERVICIO INT,
   @FECHA_HORA DATETIME,
   @ESTADO NVARCHAR(50),
   
   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    DECLARE @DURACION INT;
    DECLARE @FECHA_FIN DATETIME;

    BEGIN TRY
        -- Verificar si la cita existe
        IF NOT EXISTS (SELECT 1 FROM CITA WHERE Id_Cita = @ID_CITA)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'La cita no existe';
            RETURN;
        END

        -- Verificar si la nueva fecha de la cita es anterior a la fecha actual
        IF @FECHA_HORA < GETDATE()
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'No se pueden actualizar citas a fechas pasadas';
            RETURN;
        END

        -- Verificar si el barbero tiene el rol correcto y está activo
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_BARBERO AND Id_Rol = 2 AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 3;
            SET @ERRORDESCRIPCION = 'El barbero no existe o no está activo';
            RETURN;
        END

        -- Verificar si el usuario existe y tiene el rol correcto (Cliente)
        IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 1 AND Estado = 1)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 4;
            SET @ERRORDESCRIPCION = 'El usuario no existe o está inactivo';
            RETURN;
        END

        -- Verificar si el servicio existe y obtener su duración
        SELECT @DURACION = Duracion_Minutos FROM SERVICIO WHERE Id_Servicio = @ID_SERVICIO;

        IF @DURACION IS NULL
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 5;
            SET @ERRORDESCRIPCION = 'El servicio no existe';
            RETURN;
        END

        -- Calcular la fecha de finalización de la cita
        SET @FECHA_FIN = DATEADD(MINUTE, @DURACION, @FECHA_HORA);

        -- Verificar si el barbero tiene citas que se solapen con la nueva fecha de la cita
        IF EXISTS (
            SELECT 1 FROM CITA
            WHERE Id_Barbero = @ID_BARBERO
            AND Id_Cita <> @ID_CITA
            AND (
                (Fecha_Hora BETWEEN @FECHA_HORA AND @FECHA_FIN) -- Citas que inician dentro del rango
                OR (DATEADD(MINUTE, (SELECT Duracion_Minutos FROM SERVICIO WHERE Id_Servicio = CITA.Id_Servicio), Fecha_Hora) 
                    BETWEEN @FECHA_HORA AND @FECHA_FIN) -- Citas que terminan dentro del rango
                OR (@FECHA_HORA BETWEEN Fecha_Hora AND DATEADD(MINUTE, (SELECT Duracion_Minutos FROM SERVICIO WHERE Id_Servicio = CITA.Id_Servicio), Fecha_Hora)) -- Nueva cita dentro de una ya existente
            )
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 6;
            SET @ERRORDESCRIPCION = 'El barbero ya tiene una cita en este horario';
            RETURN;
        END

        -- Actualizar la cita
        UPDATE CITA
        SET 
            Id_Barbero = @ID_BARBERO,
            Id_Usuario = @ID_USUARIO,
            Id_Servicio = @ID_SERVICIO,
            Fecha_Hora = @FECHA_HORA,
            Estado = @ESTADO
        WHERE Id_Cita = @ID_CITA;

        -- Confirmar éxito
        SET @IDRETURN = @ID_CITA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Cita actualizada exitosamente';

    END TRY
    BEGIN CATCH
        -- Manejo de errores
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar el error en la base de datos
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Linea,
            Descripcion,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_EliminarCita--------------------------------------------
CREATE PROCEDURE [dbo].[SP_ELIMINAR_CITA]
(
   @ID_CITA BIGINT,
   @ID_USUARIO BIGINT,
   @FECHA_HORA_ACTUAL DATETIME,

   @IDRETURN INT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Verificar si la cita existe
        IF NOT EXISTS (SELECT 1 FROM CITA WHERE Id_Cita = @ID_CITA)
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'La cita no existe';
            RETURN;
        END

        -- Verificar si la cita está en estado "Pendiente"
        IF EXISTS (SELECT 1 FROM CITA WHERE Id_Cita = @ID_CITA AND Estado <> 'Pendiente')
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1; 
            SET @ERRORDESCRIPCION = 'Solo se pueden eliminar citas en estado Pendiente';
            RETURN;
        END

        -- Verificar si falta una hora o menos para la cita
        DECLARE @FECHA_HORA_CITA DATETIME;
        SELECT @FECHA_HORA_CITA = Fecha_Hora FROM CITA WHERE Id_Cita = @ID_CITA;
        
        IF DATEDIFF(MINUTE, @FECHA_HORA_ACTUAL, @FECHA_HORA_CITA) <= 60
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 4;
            SET @ERRORDESCRIPCION = 'No se puede cancelar la cita cuando falta una hora o menos para la misma';
            RETURN;
        END

        -- Eliminar la cita
        DELETE FROM CITA WHERE Id_Cita = @ID_CITA;

        -- Confirmar éxito
        SET @IDRETURN = @ID_CITA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Cita eliminada exitosamente';

    END TRY
    BEGIN CATCH
        -- Manejo de errores
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar el error en la base de datos
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Linea,
            Descripcion,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_LINE(),
            ERROR_MESSAGE(),
            GETDATE()
        );
    END CATCH
END
GO
------------------------------------------SP_OBTENER_CITAS_USUARIO--------------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_CITAS_USUARIO]
(
    @ID_USUARIO BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        -- Datos de la cita
        C.Id_Cita,
        C.Fecha_Hora,
        C.Estado,

        -- Datos del servicio
        S.Id_Servicio,
        S.Nombre AS Servicio_Nombre,
        S.Duracion_Minutos AS Servicio_Duracion,
        S.Precio AS Servicio_Precio,

        -- Datos del barbero
        B.Id_Usuario AS Id_Barbero,
        B.Nombre AS Barbero_Nombre,

        -- Datos del usuario (cliente que reservó la cita)
        U.Id_Usuario AS Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
        U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono
    FROM CITA C
    INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
    INNER JOIN USUARIO B ON C.Id_Barbero = B.Id_Usuario
    INNER JOIN USUARIO U ON C.Id_Usuario = U.Id_Usuario  
    WHERE C.Id_Usuario = @ID_USUARIO
    ORDER BY C.Fecha_Hora DESC;
END;
GO
------------------------------------------SP_OBTENER_CITAS_BARBERO--------------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_CITAS_BARBERO]
(
    @ID_BARBERO BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.Id_Cita,
        C.Fecha_Hora,
        C.Estado,
        -- Datos del cliente
        U.Id_Usuario AS Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
		U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono,		
        -- Datos del barbero
        B.Id_Usuario AS Id_Barbero,
        B.Nombre AS Barbero_Nombre,
        -- Datos del servicio
        S.Id_Servicio,
        S.Nombre AS Servicio_Nombre,
        S.Duracion_Minutos AS Servicio_Duracion,
        S.Precio AS Servicio_Precio
    FROM CITA C
    INNER JOIN USUARIO U ON C.Id_Usuario = U.Id_Usuario
	INNER JOIN USUARIO B ON C.Id_Barbero = B.Id_Usuario
    INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
    WHERE C.Id_Barbero = @ID_BARBERO
    ORDER BY C.Fecha_Hora DESC;
END
GO
------------------------------------------SP_OBTENER_CITAS_BARBERO--------------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_CITAS_POR_DIA]
(
    @FECHA DATETIME
)
AS
BEGIN
    SET NOCOUNT ON;

        SELECT 
        C.Id_Cita,
        C.Fecha_Hora,
        C.Estado,
        -- Datos del cliente
        U.Id_Usuario AS Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
		U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono,		
        -- Datos del barbero
        B.Id_Usuario AS Id_Barbero,
        B.Nombre AS Barbero_Nombre,
        -- Datos del servicio
        S.Id_Servicio,
        S.Nombre AS Servicio_Nombre,
        S.Duracion_Minutos AS Servicio_Duracion,
        S.Precio AS Servicio_Precio
    FROM CITA C
    INNER JOIN USUARIO U ON C.Id_Usuario = U.Id_Usuario
    INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
    INNER JOIN USUARIO B ON C.Id_Barbero = B.Id_Usuario
    WHERE CAST(C.Fecha_Hora AS DATE) = @FECHA
    ORDER BY C.Fecha_Hora ASC;
END
GO
------------------------------------------SP_OBTENER_CITASDIA_BARBERO--------------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_CITAS_POR_DIA_BARBERO]
(
    @ID_BARBERO BIGINT,
    @FECHA DATE
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.Id_Cita,
        C.Fecha_Hora,
        C.Estado,
        -- Datos del cliente
        U.Id_Usuario AS Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
        U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono,		
        -- Datos del barbero
        B.Id_Usuario AS Id_Barbero,
        B.Nombre AS Barbero_Nombre,
        -- Datos del servicio
        S.Id_Servicio,
        S.Nombre AS Servicio_Nombre,
        S.Duracion_Minutos AS Servicio_Duracion,
        S.Precio AS Servicio_Precio
    FROM CITA C
    INNER JOIN USUARIO U ON C.Id_Usuario = U.Id_Usuario
    INNER JOIN USUARIO B ON C.Id_Barbero = B.Id_Usuario
    INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
    WHERE C.Id_Barbero = @ID_BARBERO
    AND CAST(C.Fecha_Hora AS DATE) = @FECHA
	AND C.Estado = 'Pendiente'
    ORDER BY C.Fecha_Hora ASC;
END;
GO
------------------------------------------SP_OBTENER_TODAS_CITAS--------------------------------------------
CREATE PROCEDURE [dbo].[SP_OBTENER_TODAS_CITAS]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        C.Id_Cita,
        C.Fecha_Hora,
        C.Estado,
        -- Datos del cliente
        U.Id_Usuario AS Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
        U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono,		
        -- Datos del barbero
        B.Id_Usuario AS Id_Barbero,
        B.Nombre AS Barbero_Nombre,
        -- Datos del servicio
        S.Id_Servicio,
        S.Nombre AS Servicio_Nombre,
        S.Duracion_Minutos AS Servicio_Duracion,
        S.Precio AS Servicio_Precio
    FROM CITA C
    INNER JOIN USUARIO U ON C.Id_Usuario = U.Id_Usuario
    INNER JOIN USUARIO B ON C.Id_Barbero = B.Id_Usuario
    INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
    ORDER BY C.Fecha_Hora DESC;
END
GO
------------------------------------------SP_CONFIRMAR_CITA_TERMINADA--------------------------------------------
CREATE PROCEDURE [dbo].[SP_CONFIRMAR_CITA_TERMINADA]
(
    @ID_CITA BIGINT,
    @ID_USUARIO BIGINT,
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ROL_USUARIO INT;
    DECLARE @ID_BARBERO_CITA BIGINT;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Obtener el rol del usuario
        SELECT @ROL_USUARIO = Id_Rol FROM USUARIO WHERE Id_Usuario = @ID_USUARIO;

        -- Obtener el barbero asignado a la cita
        SELECT @ID_BARBERO_CITA = Id_Barbero FROM CITA WHERE Id_Cita = @ID_CITA;

        IF @ID_BARBERO_CITA IS NULL
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'Cita no encontrada';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar que si no es admin (rol 3), la cita le pertenezca al barbero
        IF @ROL_USUARIO <> 3 AND @ID_USUARIO <> @ID_BARBERO_CITA
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'No autorizado para confirmar esta cita';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Actualizar estado de la cita a "Terminada"
        UPDATE CITA
        SET Estado = 'Terminada'
        WHERE Id_Cita = @ID_CITA;

        SET @IDRETURN = @ID_CITA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Cita confirmada como terminada';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );

        ROLLBACK TRANSACTION;
    END CATCH
END
GO
------------------------------------------SP_MARCAR_CITA_NO_ASISTIO--------------------------------------------
CREATE PROCEDURE [dbo].[SP_MARCAR_CITA_NO_ASISTIO]
(
    @ID_CITA BIGINT,
    @ID_USUARIO BIGINT,
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ROL_USUARIO INT;
    DECLARE @ID_BARBERO_CITA BIGINT;

    BEGIN TRANSACTION;
    BEGIN TRY
        -- Obtener el rol del usuario
        SELECT @ROL_USUARIO = Id_Rol FROM USUARIO WHERE Id_Usuario = @ID_USUARIO;

        -- Obtener el barbero asignado a la cita
        SELECT @ID_BARBERO_CITA = Id_Barbero FROM CITA WHERE Id_Cita = @ID_CITA;

        IF @ID_BARBERO_CITA IS NULL
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'Cita no encontrada';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar si el usuario no es admin y la cita no le pertenece
        IF @ROL_USUARIO <> 3 AND @ID_USUARIO <> @ID_BARBERO_CITA
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'No autorizado para marcar esta cita';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar si la cita ya fue marcada como Terminada o No Asistió
        IF EXISTS (
            SELECT 1 
            FROM CITA 
            WHERE Id_Cita = @ID_CITA 
              AND Estado IN ('Terminada', 'No asistió')
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 3;
            SET @ERRORDESCRIPCION = 'La cita ya fue marcada como completada o no asistió';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Actualizar estado de la cita a "No asistió"
        UPDATE CITA
        SET Estado = 'No asistió'
        WHERE Id_Cita = @ID_CITA;

        SET @IDRETURN = @ID_CITA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Cita marcada como No asistió';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );

        ROLLBACK TRANSACTION;
    END CATCH
END
GO
------------------------------------------SP_Insertar_Factura_Productos--------------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_FACTURA_PRODUCTO]
(
    @ID_USUARIO BIGINT,
    @FECHA DATETIME,
    @ESTADO NVARCHAR(20),
    @PRODUCTOS_JSON NVARCHAR(MAX),
    @IDRETURN INT OUTPUT,
    @ERRORID INT OUTPUT,
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION; -- Iniciar transacción
        
        DECLARE @ID_FACTURA BIGINT;
        DECLARE @CALC_SUBTOTAL DECIMAL(10,2) = 0;
        DECLARE @CALC_TOTAL DECIMAL(10,2) = 0;
        DECLARE @PRODUCTOS_SIN_STOCK TABLE (Id_Producto INT, Nombre NVARCHAR(50), Stock_Actual INT, Cantidad_Solicitada INT);

        -- 1. Validación de usuario
        IF NOT EXISTS (
            SELECT 1 FROM USUARIO 
            WHERE Id_Usuario = @ID_USUARIO AND Id_Rol = 1 AND Estado = 1
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El usuario no existe o está inactivo.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Verificar existencia de productos
        IF EXISTS (
            SELECT 1 FROM OPENJSON(@PRODUCTOS_JSON)
            WITH (Id_Producto INT) AS p
            LEFT JOIN PRODUCTO pr ON p.Id_Producto = pr.Id_Producto
            WHERE pr.Id_Producto IS NULL
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'Uno o más productos no existen.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Verificar stock disponible
        INSERT INTO @PRODUCTOS_SIN_STOCK
        SELECT 
            p.Id_Producto,
            pr.Nombre,
            pr.Stock,
            p.Cantidad
        FROM OPENJSON(@PRODUCTOS_JSON)
        WITH (
            Id_Producto INT,
            Cantidad INT
        ) AS p
        JOIN PRODUCTO pr ON p.Id_Producto = pr.Id_Producto
        WHERE pr.Stock < p.Cantidad;

        IF EXISTS (SELECT 1 FROM @PRODUCTOS_SIN_STOCK)
        BEGIN
            DECLARE @ERROR_STOCK NVARCHAR(MAX) = 'Stock insuficiente para: ';
            
            SELECT @ERROR_STOCK = @ERROR_STOCK + 
                   CONCAT(Nombre, ' (Stock: ', Stock_Actual, ', Solicitado: ', Cantidad_Solicitada, '); ')
            FROM @PRODUCTOS_SIN_STOCK;
            
            SET @IDRETURN = -1;
            SET @ERRORID = 3;
            SET @ERRORDESCRIPCION = @ERROR_STOCK;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4. Calcular totales basados en productos reales
        SELECT 
            @CALC_SUBTOTAL = SUM(p.Cantidad * pr.Precio),
            @CALC_TOTAL = SUM(p.Cantidad * pr.Precio) -- Podrías agregar impuestos aquí
        FROM OPENJSON(@PRODUCTOS_JSON)
        WITH (
            Id_Producto INT,
            Cantidad INT
        ) AS p
        JOIN PRODUCTO pr ON p.Id_Producto = pr.Id_Producto;

        -- 5. Insertar factura
        INSERT INTO FACTURA (Id_Usuario, Fecha, Subtotal, Total, Estado)
        VALUES (@ID_USUARIO, @FECHA, @CALC_SUBTOTAL, @CALC_TOTAL, @ESTADO);

        SET @ID_FACTURA = SCOPE_IDENTITY();

        -- 6. Insertar productos en la factura
        INSERT INTO FACTURA_PRODUCTO (Id_Factura, Id_Producto, Cantidad, Precio)
        SELECT 
            @ID_FACTURA,
            p.Id_Producto,
            p.Cantidad,
            pr.Precio
        FROM OPENJSON(@PRODUCTOS_JSON)
        WITH (
            Id_Producto INT,
            Cantidad INT
        ) AS p
        JOIN PRODUCTO pr ON p.Id_Producto = pr.Id_Producto;

        -- 7. Actualizar stock (¡LO QUE NECESITAS!)
        UPDATE pr
        SET pr.Stock = pr.Stock - p.Cantidad
        FROM PRODUCTO pr
        JOIN OPENJSON(@PRODUCTOS_JSON)
        WITH (
            Id_Producto INT,
            Cantidad INT
        ) AS p ON pr.Id_Producto = p.Id_Producto;

        -- 8. Retornar resultados
        SET @IDRETURN = @ID_FACTURA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Factura registrada y stock actualizado exitosamente.';

        -- 9. Devolver datos completos
        SELECT 
            F.Id_Factura,
            F.Fecha AS Fecha_Factura,
            F.Subtotal,
            F.Total,
            F.Estado AS Estado_Factura,
            P.Id_Producto,
            P.Nombre AS Nombre_Producto,
            FP.Cantidad,
            FP.Precio AS Precio_Unitario,
            (FP.Cantidad * FP.Precio) AS Total_Producto,
            P.Stock AS Nuevo_Stock, -- Mostramos el nuevo stock disponible
            U.Nombre + ' ' + U.Apellido AS Nombre_Cliente,
            U.Correo_Electronico
        FROM FACTURA F
        INNER JOIN FACTURA_PRODUCTO FP ON F.Id_Factura = FP.Id_Factura
        INNER JOIN PRODUCTO P ON FP.Id_Producto = P.Id_Producto
        INNER JOIN USUARIO U ON F.Id_Usuario = U.Id_Usuario
        WHERE F.Id_Factura = @ID_FACTURA;

        COMMIT TRANSACTION; -- Confirmar transacción
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS (Severidad, Store_Procesure, Descripcion, Linea, Fecha_Hora)
        VALUES (ERROR_SEVERITY(), ERROR_PROCEDURE(), ERROR_MESSAGE(), ERROR_LINE(), GETDATE());
    END CATCH
END
GO
------------------------------------------SP_Insertar_Factura_Citas--------------------------------------------
CREATE PROCEDURE [dbo].[SP_INSERTAR_FACTURA_CITA]
(
    @ID_USUARIO BIGINT,        
    @ID_CITA BIGINT,           
    @FECHA DATETIME,           
    @ESTADO NVARCHAR(20),       
    @IDRETURN INT OUTPUT,      
    @ERRORID INT OUTPUT,        
    @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT  
)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @ID_FACTURA BIGINT;
        DECLARE @CALC_SUBTOTAL DECIMAL(10,2) = 0;
        DECLARE @CALC_TOTAL DECIMAL(10,2) = 0;

        -- 1. Validación de usuario
        IF NOT EXISTS (
            SELECT 1 
            FROM USUARIO 
            WHERE Id_Usuario = @ID_USUARIO AND Estado = 1
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'El usuario no existe o está inactivo.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Validación de la cita y que le pertenezca al usuario
        IF NOT EXISTS (
            SELECT 1
            FROM CITA
            WHERE Id_Cita = @ID_CITA AND Id_Usuario = @ID_USUARIO
        )
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 2;
            SET @ERRORDESCRIPCION = 'La cita no existe o no pertenece al usuario.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Obtener el total de los servicios de la cita
        SELECT 
            @CALC_SUBTOTAL = s.Precio,
            @CALC_TOTAL = s.Precio
        FROM CITA c
        JOIN SERVICIO s ON c.Id_Servicio = s.Id_Servicio
        WHERE c.Id_Cita = @ID_CITA;

        -- 4. Insertar factura
        INSERT INTO FACTURA (Id_Usuario, Fecha, Subtotal, Total, Estado, Id_Cita)
        VALUES (@ID_USUARIO, @FECHA, @CALC_SUBTOTAL, @CALC_TOTAL, @ESTADO, @ID_CITA);

        SET @ID_FACTURA = SCOPE_IDENTITY();

        -- 5. Retornar los datos de la factura creada
        SELECT 
            f.Id_Factura,
            f.Id_Usuario,
            u.Nombre AS NombreUsuario,
            u.Correo_Electronico AS CorreoUsuario,
            f.Fecha,
            f.Subtotal,
            f.Total,
            f.Estado,
            c.Id_Cita,
            c.Fecha_Hora AS FechaCita,
            c.Id_Servicio,
            s.Nombre AS NombreServicio,
            s.Precio AS PrecioServicio
        FROM FACTURA f
        JOIN USUARIO u ON f.Id_Usuario = u.Id_Usuario
        JOIN CITA c ON f.Id_Cita = c.Id_Cita
        JOIN SERVICIO s ON c.Id_Servicio = s.Id_Servicio
        WHERE f.Id_Factura = @ID_FACTURA;

        -- 6. Finalizar
        SET @IDRETURN = @ID_FACTURA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Factura asociada a la cita registrada correctamente.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        INSERT INTO ERROR_EN_BASE_DATOS (Severidad, Store_Procesure, Descripcion, Linea, Fecha_Hora)
        VALUES (ERROR_SEVERITY(), ERROR_PROCEDURE(), ERROR_MESSAGE(), ERROR_LINE(), GETDATE());
    END CATCH
END
GO
------------------------------------------SP_confirmarPago--------------------------------------------
CREATE PROCEDURE [dbo].[SP_CONFIRMAR_PAGO]
(
   @ID_FACTURA BIGINT,
   
   @IDRETURN BIGINT OUTPUT,
   @ERRORID INT OUTPUT,
   @ERRORDESCRIPCION NVARCHAR(MAX) OUTPUT
)
AS
BEGIN
    BEGIN TRY
        -- Validar que la factura exista y esté en estado 'Pendiente'
        IF NOT EXISTS (SELECT 1 FROM FACTURA WHERE Id_Factura = @ID_FACTURA AND Estado = 'Pendiente')
        BEGIN
            SET @IDRETURN = -1;
            SET @ERRORID = 1;
            SET @ERRORDESCRIPCION = 'La factura no existe o ya ha sido pagada.';
            RETURN;
        END

        -- Actualizar la factura a estado "Pagado"
        UPDATE FACTURA 
        SET Estado = 'Pagado',
            Fecha = GETDATE() -- Se actualiza la fecha de pago
        WHERE Id_Factura = @ID_FACTURA;

        -- Registrar éxito de la transacción
        SET @IDRETURN = @ID_FACTURA;
        SET @ERRORID = 0;
        SET @ERRORDESCRIPCION = 'Pago confirmado exitosamente.';

    END TRY
    BEGIN CATCH
        -- Manejo de errores
        SET @IDRETURN = -1;
        SET @ERRORID = ERROR_NUMBER();
        SET @ERRORDESCRIPCION = ERROR_MESSAGE();

        -- Registrar el error en la base de datos
        INSERT INTO ERROR_EN_BASE_DATOS
        (
            Severidad,
            Store_Procesure,
            Descripcion,
            Linea,
            Fecha_Hora
        )
        VALUES
        (
            ERROR_SEVERITY(),
            ERROR_PROCEDURE(),
            ERROR_MESSAGE(),
            ERROR_LINE(),
            GETDATE()
        );
    END CATCH
END
GO

------------------------------------------SP_ListarFacturasUsuarioCitas--------------------------------------------
CREATE PROCEDURE [dbo].[SP_LISTAR_FACTURAS_USUARIO_CITAS]
(
    @IdUsuario BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        -- Info de la factura
        F.Id_Factura,
        F.Fecha,
        F.Subtotal,
        F.Total,
        F.Estado,

        -- Info de la cita
        C.Id_Cita,
        C.Fecha_Hora AS Fecha_Cita,

        -- Info del servicio
        S.Nombre AS Servicio,
        S.Descripcion,
        S.Duracion_Minutos,
        S.Precio AS Precio_Servicio,

        -- Info del cliente
        U.Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
        U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono

    FROM FACTURA F
    INNER JOIN CITA C ON F.Id_Cita = C.Id_Cita
    INNER JOIN SERVICIO S ON C.Id_Servicio = S.Id_Servicio
    INNER JOIN USUARIO U ON F.Id_Usuario = U.Id_Usuario
    WHERE F.Id_Usuario = @IdUsuario
    ORDER BY F.Fecha DESC
END
GO
------------------------------------------SP_ListarFacturasUsuarioProductos--------------------------------------------
CREATE PROCEDURE [dbo].[SP_LISTAR_FACTURAS_USUARIO_PRODUCTOS]
(
    @Id_Usuario BIGINT
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        F.Id_Factura,
        F.Fecha AS Fecha_Factura,
        F.Subtotal,
        F.Total,
        F.Estado AS Estado_Factura,

        -- Info del producto
        FP.Id_Producto,
        P.Nombre AS Nombre_Producto,
        FP.Cantidad,
        FP.Precio AS Precio_Unitario,
        (FP.Cantidad * FP.Precio) AS Total_Producto,

        -- Info del usuario (cliente)
        U.Id_Usuario,
        U.Nombre AS Usuario_Nombre,
        U.Apellido AS Usuario_Apellido,
        U.Correo_Electronico AS Usuario_Correo,
        U.Telefono AS Usuario_Telefono

    FROM FACTURA F
    INNER JOIN FACTURA_PRODUCTO FP ON F.Id_Factura = FP.Id_Factura
    INNER JOIN PRODUCTO P ON FP.Id_Producto = P.Id_Producto
    INNER JOIN USUARIO U ON F.Id_Usuario = U.Id_Usuario
    WHERE F.Id_Usuario = @Id_Usuario
    ORDER BY F.Fecha DESC;
END
GO