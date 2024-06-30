USE taskify;
INSERT [dbo].[Users] ([Id], [FirstName], [LastName], [Address], [ImageUrl], [ImageLocalPathUrl], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'1', N'Admin', N'Admin', NULL, NULL, NULL, N'admin@admin.com', N'admin@admin.com', N'admin@admin.com', N'admin@admin.com', 0, N'admin@admin.com', N'admin@admin.com', N'admin@admin.com', N'0000000000', 0, 0, NULL, 1, 0)

INSERT INTO dbo.Activities (Title, Description) VALUES(N'Created', N'Created');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Updated', N'Updated');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Duplicated', N'Duplicated');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Uploaded', N'Uploaded');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Deleted', N'Deleted');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Updated Status', N'Updated Status');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Signed', N'Signed');
INSERT INTO dbo.Activities (Title, Description) VALUES(N'Unsigned', N'Unsigned');



INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Meeting', N'Meeting');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Note', N'Note');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Project', N'Project');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Status', N'Status');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Tag', N'Tag');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Task', N'Task');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Todo', N'Todo');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Activity', N'Activity');
INSERT INTO dbo.ActivityTypes (Title, Description) VALUES(N'Color', N'Color');

INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'warning', N'#ffc107', N'1', 1, N'Warning');
INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'primary', N'#0d6efd', N'1', 1, N'Primary');
INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'secondary', N'#6c757d', N'1', 1, N'Secondary');
INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'success', N'#198754', N'1', 1, N'Success');
INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'danger', N'#dc3545', N'1', 1, N'Danger');
INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'info', N'#0dcaf0', N'1', 1, N'Info');
INSERT INTO dbo.Colors(Title, ColorCode, UserId, IsDefault, Description) VALUES(N'dark', N'#212529', N'1', 1, N'Dark');



INSERT INTO dbo.Status(ColorId, Title, Description, UserId, IsDefault) VALUES (1, N'In Review', N'Warning', N'1', 1);
INSERT INTO dbo.Status(ColorId, Title, Description, UserId, IsDefault) VALUES (6, N'On Going', N'Infor', N'1', 1);
INSERT INTO dbo.Status(ColorId, Title, Description, UserId, IsDefault) VALUES (2, N'Started', N'Primary', N'1', 1);
INSERT INTO dbo.Status(ColorId, Title, Description, UserId, IsDefault) VALUES (5, N'Default', N'Danger', N'1', 1);

INSERT INTO dbo.Priorities(Title, ColorId, UserId, IsDefault) VALUES(N'Default', 7, N'1', 1);
INSERT INTO dbo.Priorities(Title, ColorId, UserId, IsDefault) VALUES(N'Low', 4, N'1', 1);
INSERT INTO dbo.Priorities(Title, ColorId, UserId, IsDefault) VALUES(N'Medium', 1, N'1', 1);
INSERT INTO dbo.Priorities(Title, ColorId, UserId, IsDefault) VALUES(N'High', 5, N'1', 1);

INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'WEBDESIGN', N'WEBDESIGN', 2, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'BOOKING AND RESERVATION', N'BOOKING AND RESERVATION', 7, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'LEARNING AND EDUCATION', N'LEARNING AND EDUCATION', 6, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'PROJECT MANAGEMENT', N'PROJECT MANAGEMENT', 1, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'CONTENT MANAGEMENT', N'CONTENT MANAGEMENT', 5, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'SOCIAL NETWORKING', N'SOCIAL NETWORKING', 4, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'E-COMMERCE', N'E-COMMERCE', 3, N'1', 1);
INSERT INTO dbo.Tags (Title, Description, ColorId, UserId, IsDefault) VALUES(N'WEB DEVELOPMENT', N'WEB DEVELOPMENT', 2, N'1', 1);
-- DBCC CHECKIDENT ('Status', RESEED, 0);
