import random
import string
from sqlalchemy import Connection, create_engine
from sqlalchemy.sql import text
from time import sleep

DEV_USER_ID = 'aea5a17c-b73e-43a5-be7b-8352a2adce5d'
DEV_OTHER_USER_IN_WORKSPACE_ID = '7e6756cb-d233-4560-bbd4-43c6b25ad675'
DEV_WORKSPACE_ID = '2286c703-e564-4cd2-9df1-25924abb9900'

connection_string = "postgresql://postgres:postgres@localhost:5432/slack_clone"
engine = create_engine(connection_string)
rand_string = lambda l: ''.join(random.choices(string.ascii_uppercase + string.digits, k=l))

def insert_channels(conn: Connection, num_channels):
    statement = text("""INSERT INTO "Channels" ("CreatedById", "Description", "Name", "Topic", "WorkspaceId") VALUES (:created_by_id, :description, :name, :topic, :workspace_id)""")
    for _ in range(num_channels):
        new_row = {
            "created_by_id": DEV_USER_ID, 
            "description": "test-description",
            "name": "test-channel" + rand_string(10),
            "topic": "test-topic" + rand_string(10),
            "workspace_id": DEV_WORKSPACE_ID
        }
        conn.execute(statement, new_row)
    conn.commit()
    statement = text('SELECT "Id" FROM "Channels" ORDER BY "CreatedAt" DESC LIMIT :num_channels')
    inserted = conn.execute(statement, {"num_channels": num_channels})
    inserted_ids = list(str(row[0]) for row in inserted)
    statement = text("""INSERT INTO "ChannelMembers" ("ChannelId", "UserId", "Starred", "WorkspaceId") VALUES (:channel_id, :user_id, :starred, :workspace_id)""")
    for id in inserted_ids:
        member = {
            "channel_id": id,
            "user_id": DEV_USER_ID,
            "starred": True,
            "workspace_id": DEV_WORKSPACE_ID 
        }
        conn.execute(statement, member)
    conn.commit()

def insert_direct_message_groups(conn: Connection, num_groups):
    statement = text("""INSERT INTO "DirectMessageGroups" ("WorkspaceId", "Size") VALUES (:workspace_id, :size)""")    
    for _ in range(num_groups):
        new_row = {
            "workspace_id": DEV_WORKSPACE_ID,
            "size": 0
        }
        conn.execute(statement, new_row)
    conn.commit()
    statement = text('SELECT "Id" FROM "DirectMessageGroups" WHERE "Size" = 0')
    inserted = conn.execute(statement)
    inserted_ids = list(str(row[0]) for row in inserted)
    statement = text("""INSERT INTO "DirectMessageGroupMembers" ("DirectMessageGroupId", "UserId", "Starred", "WorkspaceId") VALUES (:group_id, :user_id, :starred, :workspace_id)""")
    for id in inserted_ids:
        member1 = {
            "group_id": id,
            "user_id": DEV_USER_ID,
            "starred": True,
            "workspace_id": DEV_WORKSPACE_ID 
        }
        member2 = {
            "group_id": id,
            "user_id": DEV_OTHER_USER_IN_WORKSPACE_ID,
            "starred": True,
            "workspace_id": DEV_WORKSPACE_ID 
        }
        conn.execute(statement, member1)
        conn.execute(statement, member2)
        conn.execute(text('UPDATE "DirectMessageGroups" SET "Size" = 2 WHERE "Id" = :id'), { "id": id })
    conn.commit()

def insert_stars(conn: Connection, num_stars):
    # get dev users channel and dmg memberships in the workspace
    # for every other unstarred membership:
        # star it by:
            # updating the membership starred column
            # creating a new Star row
    statement = text('SELECT "ChannelId" FROM "ChannelMembers" WHERE "WorkspaceId" = :workspaceId AND "UserId" = :userId')
    params = { "workspaceId": DEV_WORKSPACE_ID, "userId": DEV_USER_ID}
    rows = conn.execute(statement, params)
    channel_ids = list(str(row[0]) for row in rows)
    statement = text('SELECT "DirectMessageGroupId" FROM "DirectMessageGroupMembers" WHERE "WorkspaceId" = :workspaceId AND "UserId" = :userId')
    rows = conn.execute(statement, params)
    dm_group_ids = list(str(row[0]) for row in rows)
    for i in range(min(len(channel_ids) + len(dm_group_ids), num_stars)):
        id = None
        channel = False
        if i & 1 or i // 2 >= len(channel_ids):
            id = dm_group_ids[i // 2]
        else:
            id = channel_ids[i // 2]
            channel = True
        s = 'UPDATE ' + ('"ChannelMembers"' if channel else '"DirectMessageGroupMembers"') + ' SET "Starred" = TRUE WHERE "WorkspaceId" = :workspaceId AND "UserId" = :userId'
        statement = text(s)
        conn.execute(statement, params)
        s = 'INSERT INTO "Stars" ("UserId", ' + ('"ChannelId",' if channel else '"DirectMessageGroupId",') + ' "WorkspaceId") VALUES (:userId, :id, :workspaceId)'
        statement = text(s)
        conn.execute(statement, { **params, "id": id })
        conn.commit()
        sleep(1)
        

with engine.connect() as conn:
    # insert_channels(conn, 20)
    # insert_direct_message_groups(conn, 20)
    insert_stars(conn, 50)