-- Create Matches table
CREATE TABLE IF NOT EXISTS "Matches" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "TournamentId" UUID NOT NULL,
    "Round" INTEGER NOT NULL,
    "Position" INTEGER NOT NULL,
    "Player1Id" UUID,
    "Player2Id" UUID,
    "Score1" INTEGER DEFAULT 0,
    "Score2" INTEGER DEFAULT 0,
    "WinnerId" UUID,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Pending',
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("TournamentId") REFERENCES "Tournaments"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("Player1Id") REFERENCES "Players"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("Player2Id") REFERENCES "Players"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("WinnerId") REFERENCES "Players"("Id") ON DELETE SET NULL
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_Matches_TournamentId" ON "Matches"("TournamentId");
CREATE INDEX IF NOT EXISTS "IX_Matches_Round" ON "Matches"("TournamentId", "Round");
CREATE INDEX IF NOT EXISTS "IX_Matches_Player1Id" ON "Matches"("Player1Id");
CREATE INDEX IF NOT EXISTS "IX_Matches_Player2Id" ON "Matches"("Player2Id");
CREATE INDEX IF NOT EXISTS "IX_Matches_WinnerId" ON "Matches"("WinnerId");

