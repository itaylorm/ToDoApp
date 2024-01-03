CREATE PROCEDURE [dbo].[spTodos_Delete]
	@AssignedTo int,
	@TodoId int
AS
BEGIN
	DELETE FROM [dbo].[Todos]
	WHERE [AssignedTo] = @AssignedTo AND [Id] = @TodoId
END
