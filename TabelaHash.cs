namespace notepad;

class Tabela {
  private string chave;
  private string valor;
  private Tabela proximo;
  public Tabela(string c, string v, Tabela prox) {
    chave = c;
    valor = v;
    proximo = prox;
  }
  public void setChave(string c) => chave = c;
  public void setValor(string v)  => valor = v;
  public void setProximo(Tabela p) => proximo = p;
  public string getValor() => valor;
  public string getChave() => chave;
  public Tabela getProximo() => proximo;
}
public class TabelaHash {
  private int tamanho;
  private Tabela[] items;
  public TabelaHash(int tam) {
    tamanho = tam;
    items = new Tabela[tam];
  }
  public int Hash(string chave)  {
    ulong hash = 5381;
    foreach(char c in chave) {
      hash = (hash << 5) + c;
    }
    return (int)(hash % (ulong)tamanho);
  }
  public void Inserir(string chave, string valor) {
    int pos = Hash(chave);
    Tabela t = new Tabela(chave, valor, items[pos]);
    items[pos] = t;
  }
  public string buscar(string chave) {
    int pos = Hash(chave);
    Tabela aux = items[pos];
    while(aux != null) {
      if(aux.getChave() == chave) return aux.getValor();
      aux = aux.getProximo();
    }
     return "";
  }
}