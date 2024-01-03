CREATE PROCEDURE [dbo].[spTodos_Create]
	@Task NVARCHAR(50),
	@AssignedTo INT,
	@Id INT OUTPUT
AS
BEGIN
	INSERT INTO [dbo].[Todos] ([Task], [AssignedTo])
	VALUES (@Task, @AssignedTo)

	SET @Id = SCOPE_IDENTITY()
END
