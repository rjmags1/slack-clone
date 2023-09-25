import random
import string
from sqlalchemy import Connection, create_engine
from sqlalchemy.sql import text

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

with engine.connect() as conn:
    insert_channels(conn, 20)
    insert_direct_message_groups(conn, 20)
