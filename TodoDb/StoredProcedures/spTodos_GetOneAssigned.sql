CREATE PROCEDURE [dbo].[spTodos_GetOneAssigned]
	@AssignedTo INT,
	@TodoId INT
AS
BEGIN
SELECT [Id], [Task], [AssignedTo], [IsComplete] FROM [dbo].[Todos]
	WHERE [AssignedTo] = @AssignedTo AND [Id] = @TodoId
END
