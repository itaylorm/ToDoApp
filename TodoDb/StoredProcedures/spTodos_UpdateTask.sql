CREATE PROCEDURE [dbo].[spTodos_UpdateTask]
	@Task nvarchar(50),
	@AssignedTo int,
	@TodoId int
AS
BEGIN
	UPDATE [dbo].[Todos]
	SET [Task] = @Task
	WHERE [AssignedTo] = @AssignedTo AND [Id] = @TodoId
END
