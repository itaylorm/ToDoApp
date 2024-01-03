CREATE PROCEDURE [dbo].[spTodos_UpdateComplete]
	@AssignedTo int,
	@TodoId int
AS
BEGIN
	UPDATE [dbo].[Todos]
	SET IsComplete = 1
	WHERE [AssignedTo] = @AssignedTo AND [Id] = @TodoId
END

