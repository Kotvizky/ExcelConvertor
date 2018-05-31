cd C:\Users\IKotvytskyi\Documents\SqlBaseScript\
sqlcmd -S 10.1.32.56  -i get_sp_sctript.sql -o script_sp.sql
sqlcmd -S 10.1.32.56  -i get_fn_sctript.sql -o script_fn.sql