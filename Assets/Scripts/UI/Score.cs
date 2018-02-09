public class Score 
{
	public int score;
	public int score_pepsi;
	public int score_cola;

	//Constructors
	public Score(int score){
		this.score = score;
	}

	public Score(int score_cola, int score_pepsi){
		this.score_cola = score_cola;
		this.score_pepsi = score_pepsi;
	}
}

