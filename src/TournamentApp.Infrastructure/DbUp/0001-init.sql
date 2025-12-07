-- Create schema
CREATE SCHEMA IF NOT EXISTS public;

-- Create Players table
CREATE TABLE IF NOT EXISTS "Players" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(255) NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create Tournaments table
CREATE TABLE IF NOT EXISTS "Tournaments" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Name" VARCHAR(255) NOT NULL,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Draft',
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create TournamentPlayers junction table
CREATE TABLE IF NOT EXISTS "TournamentPlayers" (
    "TournamentId" UUID NOT NULL,
    "PlayerId" UUID NOT NULL,
    PRIMARY KEY ("TournamentId", "PlayerId"),
    FOREIGN KEY ("TournamentId") REFERENCES "Tournaments"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PlayerId") REFERENCES "Players"("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_TournamentPlayers_TournamentId" ON "TournamentPlayers"("TournamentId");
CREATE INDEX IF NOT EXISTS "IX_TournamentPlayers_PlayerId" ON "TournamentPlayers"("PlayerId");

